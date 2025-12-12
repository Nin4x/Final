using System.Net;

namespace LoanApi.Application.Exceptions;

public sealed class BadRequestException : DomainException
{
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public BadRequestException(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message, HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
}
