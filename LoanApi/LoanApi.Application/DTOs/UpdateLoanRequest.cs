namespace LoanApi.Application.DTOs;

public record UpdateLoanRequest(
    string BorrowerName,
    decimal Amount,
    decimal InterestRate,
    int TermMonths);
