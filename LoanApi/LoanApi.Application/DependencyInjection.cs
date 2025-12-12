using LoanApi.Application.Interfaces;
using LoanApi.Application.Services;
using LoanApi.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<CreateLoanRequestValidator>();
        services.AddScoped<UpdateLoanStatusRequestValidator>();
        return services;
    }
}
