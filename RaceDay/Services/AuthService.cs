
using Microsoft.AspNetCore.Identity;

namespace RaceDay.API.Services;

public class AuthService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new object(), password);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            new object(),
            hashedPassword,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}

