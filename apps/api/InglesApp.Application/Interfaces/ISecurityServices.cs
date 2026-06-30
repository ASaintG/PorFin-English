using InglesApp.Application.DTOs.Auth;

namespace InglesApp.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(string userId, string email);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
