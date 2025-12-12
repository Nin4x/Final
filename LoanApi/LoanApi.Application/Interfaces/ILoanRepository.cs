using LoanApi.Domain.Entities;

namespace LoanApi.Application.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Loan> CreateAsync(Loan loan, CancellationToken cancellationToken = default);

    Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default);

    Task DeleteAsync(Loan loan, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Loan>> QueryAllAsync(CancellationToken cancellationToken = default);
}
