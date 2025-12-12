using LoanApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureLoans(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var userBuilder = modelBuilder.Entity<User>();

        userBuilder.ToTable("Users");
        userBuilder.HasKey(user => user.Id);

        userBuilder.Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(50);

        userBuilder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(256);

        userBuilder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        userBuilder.Property(user => user.CreatedOnUtc).IsRequired();
        userBuilder.Property(user => user.UpdatedOnUtc);

        userBuilder.HasIndex(user => user.Username).IsUnique();
        userBuilder.HasIndex(user => user.Email).IsUnique();
    }

    private static void ConfigureLoans(ModelBuilder modelBuilder)
    {
        var loanBuilder = modelBuilder.Entity<Loan>();

        loanBuilder.ToTable("Loans");
        loanBuilder.HasKey(loan => loan.Id);

        loanBuilder.Property(loan => loan.BorrowerName)
            .IsRequired()
            .HasMaxLength(200);

        loanBuilder.Property(loan => loan.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        loanBuilder.Property(loan => loan.InterestRate)
            .IsRequired()
            .HasPrecision(5, 2);

        loanBuilder.Property(loan => loan.TermMonths).IsRequired();
        loanBuilder.Property(loan => loan.Status).IsRequired();
        loanBuilder.Property(loan => loan.CreatedOnUtc).IsRequired();
        loanBuilder.Property(loan => loan.UpdatedOnUtc);

        loanBuilder
            .HasOne(loan => loan.User)
            .WithMany(user => user.Loans)
            .HasForeignKey(loan => loan.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
