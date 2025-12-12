namespace LoanApi.Application.DTOs;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    int Age,
    decimal MonthlyIncome);
