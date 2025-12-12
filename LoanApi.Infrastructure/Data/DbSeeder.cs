using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoanApi.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await context.Database.MigrateAsync();

        if (!context.Users.Any(u => u.Role == Roles.Accountant))
        {
            var username = configuration.GetValue<string>("Seed:AccountantUsername") ?? "accountant";
            var email = configuration.GetValue<string>("Seed:AccountantEmail") ?? "accountant@loanapi.local";
            var password = configuration.GetValue<string>("Seed:AccountantPassword") ?? "ChangeMe123!";

            var accountant = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "System",
                LastName = "Accountant",
                Username = username,
                Email = email,
                MonthlyIncome = 0,
                Age = 30,
                Role = Roles.Accountant,
                PasswordHash = hasher.Hash(password)
            };

            context.Users.Add(accountant);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded accountant user {Username}", username);
        }
    }
}
