namespace LoanApi.Application.DTOs;

public record CreateLoanRequest(
    string BorrowerName,
    decimal Amount,
    decimal InterestRate,
    int TermMonths);
