using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs.Loans;

public class LoanResponse
{
    public Guid Id { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Period { get; set; }
    public LoanStatus Status { get; set; }
    public Guid UserId { get; set; }
}
