using InglesApp.Application.DTOs.Auth;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required.");

        if (request.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            LastActive = DateTime.UtcNow
        };

        var created = await _userRepository.CreateAsync(user, cancellationToken);
        var token = _tokenService.GenerateToken(created.Id, created.Email);

        return new AuthResponse(token, created.Id, created.Email, created.CurrentLevel.ToString(), created.CurrentSubLevel, created.PlacementCompleted);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        user.LastActive = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _tokenService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Id, user.Email, user.CurrentLevel.ToString(), user.CurrentSubLevel, user.PlacementCompleted);
    }
}
