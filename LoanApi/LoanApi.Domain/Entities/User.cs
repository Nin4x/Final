using System.Collections.Generic;

namespace LoanApi.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedOnUtc { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
