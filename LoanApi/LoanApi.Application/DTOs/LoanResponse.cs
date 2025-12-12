using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs;

public record LoanResponse(
    Guid Id,
    string BorrowerName,
    decimal Amount,
    decimal InterestRate,
    int TermMonths,
    LoanStatus Status,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc,
    Guid? UserId);
