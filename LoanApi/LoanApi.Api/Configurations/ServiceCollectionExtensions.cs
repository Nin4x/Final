using FluentValidation.AspNetCore;
using LoanApi.Api.Middleware;
using LoanApi.Application;
using LoanApi.Application.Validation;
using LoanApi.Infrastructure;
using Microsoft.OpenApi.Models;

namespace LoanApi.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddScoped<ExceptionHandlingMiddleware>();

        services
            .AddControllers();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Loan API",
                Version = "v1",
                Description = "Sample loan management API using Clean Architecture"
            });
        });

        return services;
    }
}
