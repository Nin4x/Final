using FluentValidation;
using LoanApi.Application.DTOs;

namespace LoanApi.Application.Validation;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(request => request.BorrowerName)
            .NotEmpty().WithMessage("Borrower name is required.")
            .MaximumLength(200);

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("Loan amount must be greater than zero.");

        RuleFor(request => request.Currency)
            .IsInEnum().WithMessage("Currency is required and must be valid.");

        RuleFor(request => request.PeriodMonths)
            .GreaterThan(0).WithMessage("Loan period must be greater than zero months.");

        RuleFor(request => request.Type)
            .IsInEnum().WithMessage("Loan type must be valid.");

        RuleFor(request => request.InterestRate)
            .GreaterThan(0).WithMessage("Interest rate must be greater than zero.");
    }
}
