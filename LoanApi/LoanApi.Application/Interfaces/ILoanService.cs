using LoanApi.Application.DTOs;

namespace LoanApi.Application.Interfaces;

public interface ILoanService
{
    Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoanDto?> UpdateStatusAsync(Guid id, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default);
}
