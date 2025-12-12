using FluentValidation;
using LoanApi.Application.DTOs.Auth;

namespace LoanApi.Application.Validation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Age).GreaterThan(0);
        RuleFor(x => x.MonthlyIncome).GreaterThanOrEqualTo(0);
    }
}
