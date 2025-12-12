using LoanApi.Application.DTOs;

namespace LoanApi.Application.Interfaces;

public interface ILoanService
{
    Task<LoanResponse> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LoanResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LoanResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoanResponse?> UpdateAsync(Guid id, UpdateLoanRequest request, CancellationToken cancellationToken = default);
    Task<LoanResponse?> UpdateStatusAsync(Guid id, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default);
}
