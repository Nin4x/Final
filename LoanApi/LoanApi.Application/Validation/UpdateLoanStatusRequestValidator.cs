using FluentValidation;
using LoanApi.Application.DTOs;

namespace LoanApi.Application.Validation;

public class UpdateLoanStatusRequestValidator : AbstractValidator<UpdateLoanStatusRequest>
{
    public UpdateLoanStatusRequestValidator()
    {
        RuleFor(request => request.Status)
            .IsInEnum().WithMessage("Unknown loan status.");
    }
}
