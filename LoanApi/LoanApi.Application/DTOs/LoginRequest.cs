namespace LoanApi.Application.DTOs;

public record LoginRequest(
    string UsernameOrEmail,
    string Password);
