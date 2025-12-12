using System.Net;
using System.Text.Json;
using FluentValidation;
using LoanApi.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            _logger.LogError(ex, "Unhandled exception occurred while processing {RequestPath}", context.Request.Path);
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title, detail, errors) = ex switch
        {
            BadRequestException badRequestException =>
                (HttpStatusCode.BadRequest, "Bad Request", badRequestException.Message, badRequestException.Errors),
            NotFoundException notFoundException =>
                (HttpStatusCode.NotFound, "Not Found", notFoundException.Message, default(IReadOnlyDictionary<string, string[]>?)),
            ForbiddenException forbiddenException =>
                (HttpStatusCode.Forbidden, "Forbidden", forbiddenException.Message, default(IReadOnlyDictionary<string, string[]>?)),
            ValidationException validationException =>
                (
                    HttpStatusCode.BadRequest,
                    "Validation failed.",
                    "One or more validation errors occurred.",
                    validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray())
                ),
            UnauthorizedAccessException unauthorizedAccessException =>
                (HttpStatusCode.Forbidden, "Forbidden", unauthorizedAccessException.Message, default(IReadOnlyDictionary<string, string[]>?)),
            _ =>
                (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred. Please contact support if the issue persists.", default(IReadOnlyDictionary<string, string[]>?))
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is not null && errors.Any())
        {
            problemDetails.Extensions["errors"] = errors;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
