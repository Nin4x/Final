using FluentValidation;
using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Validation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50);

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(request => request.Age)
            .GreaterThan(0).WithMessage("Age must be greater than zero.");

        RuleFor(request => request.MonthlyIncome)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly income cannot be negative.");

        RuleFor(request => request.Role)
            .IsInEnum().WithMessage($"Role must be either {UserRole.User} or {UserRole.Accountant}.");
    }
}
