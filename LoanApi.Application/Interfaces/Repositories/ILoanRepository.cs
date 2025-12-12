using LoanApi.Domain.Entities;

namespace LoanApi.Application.Interfaces.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id);
    Task<IEnumerable<Loan>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(Loan loan);
}
