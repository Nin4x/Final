namespace LoanApi.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
