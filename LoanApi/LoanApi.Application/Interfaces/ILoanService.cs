using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Interfaces;

public interface ILoanService
{
    Task<LoanResponse> CreateAsync(Guid currentUserId, UserRole currentUserRole, CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LoanResponse>> GetAllAsync(Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default);
    Task<LoanResponse?> GetAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default);
    Task<LoanResponse?> UpdateAsync(Guid id, Guid currentUserId, UserRole currentUserRole, UpdateLoanRequest request, CancellationToken cancellationToken = default);
    Task<LoanResponse?> UpdateStatusAsync(Guid id, Guid currentUserId, UserRole currentUserRole, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default);
}
