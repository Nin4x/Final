namespace LoanApi.Infrastructure.Auth;

public class AccountantUserOptions
{
    public const string SectionName = "AccountantUser";

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
