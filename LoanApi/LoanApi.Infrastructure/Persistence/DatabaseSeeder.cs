using System;
using System.Threading;
using LoanApi.Application.Interfaces;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;
using LoanApi.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LoanApi.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAccountantUserAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AccountantUserOptions>>().Value;
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(options.Username)
            || string.IsNullOrWhiteSpace(options.Email)
            || string.IsNullOrWhiteSpace(options.Password))
        {
            return;
        }

        var existingUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Username == options.Username || user.Email == options.Email,
                cancellationToken);

        if (existingUser is not null)
        {
            return;
        }

        var accountantUser = new User
        {
            Id = Guid.NewGuid(),
            Username = options.Username.Trim(),
            Email = options.Email.Trim(),
            Role = UserRole.Accountant,
            IsBlocked = false,
            CreatedOnUtc = dateTimeProvider.UtcNow
        };

        accountantUser.PasswordHash = passwordHasher.HashPassword(accountantUser, options.Password);

        await context.Users.AddAsync(accountantUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
