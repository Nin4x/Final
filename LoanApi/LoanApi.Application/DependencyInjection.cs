using FluentValidation;
using LoanApi.Application.Interfaces;
using LoanApi.Application.Services;
using LoanApi.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}
