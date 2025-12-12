using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs;

public record CreateLoanRequest(
    string BorrowerName,
    decimal Amount,
    LoanCurrency Currency,
    int PeriodMonths,
    LoanType Type,
    decimal InterestRate);
