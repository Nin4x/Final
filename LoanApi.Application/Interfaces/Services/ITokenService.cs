using LoanApi.Domain.Entities;

namespace LoanApi.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
