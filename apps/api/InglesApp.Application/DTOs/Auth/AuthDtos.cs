using InglesApp.Application.DTOs.Auth;

namespace InglesApp.Application.DTOs.Auth;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string Token,
    string UserId,
    string Email,
    string CurrentLevel,
    string CurrentSubLevel,
    bool PlacementCompleted);
