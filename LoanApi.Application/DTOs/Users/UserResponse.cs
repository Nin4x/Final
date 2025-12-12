namespace LoanApi.Application.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public bool IsBlocked { get; set; }
    public string Role { get; set; } = string.Empty;
}
