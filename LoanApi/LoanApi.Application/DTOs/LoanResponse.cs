using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs;

public record LoanResponse(
    Guid Id,
    string BorrowerName,
    decimal Amount,
    LoanCurrency Currency,
    int PeriodMonths,
    LoanType Type,
    decimal InterestRate,
    LoanStatus Status,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc,
    Guid? UserId);
