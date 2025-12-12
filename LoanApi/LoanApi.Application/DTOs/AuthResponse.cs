namespace LoanApi.Application.DTOs;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresOnUtc,
    UserResponse User);
