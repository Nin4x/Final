using LoanApi.Application.Interfaces;
using LoanApi.Infrastructure.Auth;
using LoanApi.Infrastructure.Persistence;
using LoanApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoanDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("LoanDatabase");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("LoanApi");
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
