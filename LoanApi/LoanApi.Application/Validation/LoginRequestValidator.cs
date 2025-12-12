using FluentValidation;
using LoanApi.Application.DTOs;

namespace LoanApi.Application.Validation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.UsernameOrEmail)
            .NotEmpty().WithMessage("Username or email is required.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
