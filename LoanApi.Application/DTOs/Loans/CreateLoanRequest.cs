using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs.Loans;

public class CreateLoanRequest
{
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Period { get; set; }
}
