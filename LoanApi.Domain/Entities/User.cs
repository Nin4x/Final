using LoanApi.Domain.Enums;

namespace LoanApi.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public bool IsBlocked { get; set; } = false;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.User;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

public static class Roles
{
    public const string User = "User";
    public const string Accountant = "Accountant";
}
