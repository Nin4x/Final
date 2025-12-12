using LoanApi.Application.DTOs.Users;

namespace LoanApi.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse?> GetAsync(Guid id, Guid requesterId, string requesterRole);
}
