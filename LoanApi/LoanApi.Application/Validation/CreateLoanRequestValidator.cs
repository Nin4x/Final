using LoanApi.Application.DTOs;

namespace LoanApi.Application.Validation;

public class CreateLoanRequestValidator
{
    public ValidationResult Validate(CreateLoanRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.BorrowerName))
        {
            result.Errors.Add("Borrower name is required.");
        }

        if (request.Amount <= 0)
        {
            result.Errors.Add("Loan amount must be greater than zero.");
        }

        if (request.InterestRate <= 0)
        {
            result.Errors.Add("Interest rate must be greater than zero.");
        }

        if (request.TermMonths <= 0)
        {
            result.Errors.Add("Term must be greater than zero months.");
        }

        return result;
    }
}
