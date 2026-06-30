using InglesApp.Application.DTOs.Auth;
using InglesApp.Application.Interfaces;
using InglesApp.Application.Services;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsToken()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var tokens = new FakeTokenService();
        var service = new AuthService(users, hasher, tokens);

        var result = await service.RegisterAsync(new RegisterRequest("test@example.com", "secret12"));

        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("fake-token", result.Token);
        Assert.Single(users.Users);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var tokens = new FakeTokenService();
        var service = new AuthService(users, hasher, tokens);

        await service.RegisterAsync(new RegisterRequest("user@test.com", "password1"));
        var login = await service.LoginAsync(new LoginRequest("user@test.com", "password1"));

        Assert.Equal("fake-token", login.Token);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            user.Id = Guid.NewGuid().ToString();
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hash:{password}";
        public bool Verify(string password, string hash) => hash == $"hash:{password}";
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(string userId, string email) => "fake-token";
    }
}
