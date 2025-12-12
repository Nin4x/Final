using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Validation;

public class UpdateLoanStatusRequestValidator
{
    public ValidationResult Validate(UpdateLoanStatusRequest request)
    {
        var result = new ValidationResult();

        if (!Enum.IsDefined(typeof(LoanStatus), request.Status))
        {
            result.Errors.Add("Unknown loan status.");
        }

        return result;
    }
}
