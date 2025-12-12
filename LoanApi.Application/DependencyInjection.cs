using FluentValidation;
using FluentValidation.AspNetCore;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Application.Mapping;
using LoanApi.Application.Services;
using LoanApi.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoanService, LoanService>();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}
