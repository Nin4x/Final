using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Infrastructure.Auth;
using LoanApi.Infrastructure.Configuration;
using LoanApi.Infrastructure.Data;
using LoanApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LoanApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=loanapi.db";
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        return services;
    }
}
