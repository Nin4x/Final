using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using LoanApi.Application.Validation;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateLoanRequestValidator _createValidator;
    private readonly UpdateLoanStatusRequestValidator _statusValidator;

    public LoanService(
        ILoanRepository repository,
        IDateTimeProvider dateTimeProvider,
        CreateLoanRequestValidator createValidator,
        UpdateLoanStatusRequestValidator statusValidator)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
    }

    public async Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = _createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validationResult.Errors));
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = request.BorrowerName.Trim(),
            Amount = request.Amount,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths,
            Status = LoanStatus.Submitted,
            CreatedOnUtc = _dateTimeProvider.UtcNow
        };

        var created = await _repository.AddAsync(loan, cancellationToken);
        return MapToDto(created);
    }

    public async Task<IReadOnlyCollection<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetAllAsync(cancellationToken);
        return loans.Select(MapToDto).ToList();
    }

    public async Task<LoanDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan is null ? null : MapToDto(loan);
    }

    public async Task<LoanDto?> UpdateStatusAsync(Guid id, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = _statusValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validationResult.Errors));
        }

        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        loan.Status = request.Status;
        loan.UpdatedOnUtc = _dateTimeProvider.UtcNow;
        await _repository.UpdateAsync(loan, cancellationToken);

        return MapToDto(loan);
    }

    private static LoanDto MapToDto(Loan loan) => new(
        loan.Id,
        loan.BorrowerName,
        loan.Amount,
        loan.InterestRate,
        loan.TermMonths,
        loan.Status,
        loan.CreatedOnUtc,
        loan.UpdatedOnUtc);
}
