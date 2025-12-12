using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default);

    Task<UserResponse> SetBlockStatusAsync(Guid id, bool isBlocked, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default);
}
