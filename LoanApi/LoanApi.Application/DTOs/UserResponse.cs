namespace LoanApi.Application.DTOs;

public record UserResponse(
    Guid Id,
    string Username,
    string Email,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc);
