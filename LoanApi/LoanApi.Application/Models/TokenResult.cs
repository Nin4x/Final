namespace LoanApi.Application.Models;

public record TokenResult(string AccessToken, DateTime ExpiresOnUtc);
