using FluentValidation;
using LoanApi.Application.DTOs.Loans;

namespace LoanApi.Application.Validation;

public class UpdateLoanRequestValidator : AbstractValidator<UpdateLoanRequest>
{
    public UpdateLoanRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty();
        RuleFor(x => x.Period).GreaterThan(0);
        RuleFor(x => x.LoanType).IsInEnum();
    }
}
