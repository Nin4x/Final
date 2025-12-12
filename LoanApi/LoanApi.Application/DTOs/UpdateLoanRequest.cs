using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs;

public record UpdateLoanRequest(
    decimal Amount,
    LoanCurrency Currency,
    int PeriodMonths,
    LoanType Type,
    decimal InterestRate,
    string BorrowerName);
