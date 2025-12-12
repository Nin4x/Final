using LoanApi.Application.Models;
using LoanApi.Domain.Entities;

namespace LoanApi.Application.Interfaces;

public interface IJwtTokenGenerator
{
    TokenResult GenerateToken(User user);
}
