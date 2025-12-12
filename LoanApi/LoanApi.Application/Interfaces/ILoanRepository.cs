using LoanApi.Domain.Entities;

namespace LoanApi.Application.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Loan>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default);
    Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
}
