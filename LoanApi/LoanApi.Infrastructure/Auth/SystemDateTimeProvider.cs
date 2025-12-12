using LoanApi.Application.Interfaces;

namespace LoanApi.Infrastructure.Auth;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
