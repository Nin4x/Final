using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Domain.Entities;
using LoanApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Loan?> GetByIdAsync(Guid id) => _context.Loans.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(Guid userId) => await _context.Loans.Where(x => x.UserId == userId).ToListAsync();

    public async Task<IEnumerable<Loan>> GetAllAsync() => await _context.Loans.AsNoTracking().ToListAsync();

    public async Task AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Loan loan)
    {
        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();
    }
}
