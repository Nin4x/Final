using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LoanApi.Infrastructure.Auth;

public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string hash, string password)
    {
        var result = _hasher.VerifyHashedPassword(null!, hash, password);
        return result == PasswordVerificationResult.Success;
    }
}
