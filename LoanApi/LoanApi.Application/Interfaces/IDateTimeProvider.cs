namespace LoanApi.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
