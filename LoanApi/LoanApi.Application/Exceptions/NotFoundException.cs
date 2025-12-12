using System.Net;

namespace LoanApi.Application.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}
