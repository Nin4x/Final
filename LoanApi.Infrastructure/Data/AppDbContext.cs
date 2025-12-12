using LoanApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Username).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired();
            entity.HasMany(x => x.Loans)
                .WithOne(x => x.User!)
                .HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(10);
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.LoanType).HasConversion<int>();
        });
    }
}
