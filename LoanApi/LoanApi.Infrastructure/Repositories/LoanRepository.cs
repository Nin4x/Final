using LoanApi.Application.Interfaces;
using LoanApi.Domain.Entities;
using LoanApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LoanDbContext _context;

    public LoanRepository(LoanDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        await _context.Loans.AddAsync(loan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return loan;
    }

    public async Task<IReadOnlyCollection<Loan>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _context.Loans.AsNoTracking().ToListAsync(cancellationToken);
        return loans;
    }

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
