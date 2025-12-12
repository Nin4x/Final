using LoanApi.Application.DTOs.Loans;

namespace LoanApi.Application.Interfaces.Services;

public interface ILoanService
{
    Task<LoanResponse> CreateAsync(CreateLoanRequest request, Guid userId, string role, bool isBlocked);
    Task<IEnumerable<LoanResponse>> GetAsync(Guid requesterId, string role);
    Task<LoanResponse?> GetByIdAsync(Guid loanId, Guid requesterId, string role);
    Task<LoanResponse> UpdateAsync(Guid loanId, UpdateLoanRequest request, Guid requesterId, string role);
    Task DeleteAsync(Guid loanId, Guid requesterId, string role);
    Task<LoanResponse> ChangeStatusAsync(Guid loanId, string status, string role);
}
