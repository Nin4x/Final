using LoanApi.Domain.Enums;

namespace LoanApi.Domain.Entities;

public class Loan
{
    public Guid Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int PeriodMonths { get; set; }
    public LoanCurrency Currency { get; set; } = LoanCurrency.USD;
    public LoanType Type { get; set; } = LoanType.Personal;
    public LoanStatus Status { get; set; } = LoanStatus.Processing;
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOnUtc { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
