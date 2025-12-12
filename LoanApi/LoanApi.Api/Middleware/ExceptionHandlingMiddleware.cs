using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

        var detail = ex switch
        {
            ValidationException validationException => string.Join(" ", validationException.Errors.Select(error => error.ErrorMessage)),
            UnauthorizedAccessException unauthorizedAccessException => unauthorizedAccessException.Message,
            ArgumentException argumentException => argumentException.Message,
            _ => "Please contact support if the issue persists."
        };

        var problemDetails = new ProblemDetails
        {
            Title = statusCode == HttpStatusCode.BadRequest ? "Validation failed." : "An unexpected error occurred.",
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = problemDetails.Status.Value;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
