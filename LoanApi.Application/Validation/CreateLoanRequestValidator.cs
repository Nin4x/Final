using FluentValidation;
using LoanApi.Application.DTOs.Loans;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Validation;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty();
        RuleFor(x => x.Period).GreaterThan(0);
        RuleFor(x => x.LoanType).IsInEnum();
    }
}
