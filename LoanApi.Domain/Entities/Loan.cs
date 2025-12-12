using LoanApi.Domain.Enums;

namespace LoanApi.Domain.Entities;

public class Loan
{
    public Guid Id { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Period { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Processing;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
