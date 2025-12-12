using LoanApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Persistence;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
    {
    }

    public DbSet<Loan> Loans => Set<Loan>();
}
