using AutoMapper;
using FluentValidation;
using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<CreateLoanRequest> _createValidator;
    private readonly IValidator<UpdateLoanRequest> _updateValidator;
    private readonly IValidator<UpdateLoanStatusRequest> _statusValidator;
    private readonly IMapper _mapper;

    public LoanService(
        ILoanRepository repository,
        IDateTimeProvider dateTimeProvider,
        IValidator<CreateLoanRequest> createValidator,
        IValidator<UpdateLoanRequest> updateValidator,
        IValidator<UpdateLoanStatusRequest> statusValidator,
        IMapper mapper)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
        _mapper = mapper;
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var loan = _mapper.Map<Loan>(request);
        loan.Id = Guid.NewGuid();
        loan.CreatedOnUtc = _dateTimeProvider.UtcNow;

        var created = await _repository.CreateAsync(loan, cancellationToken);
        return _mapper.Map<LoanResponse>(created);
    }

    public async Task<IReadOnlyCollection<LoanResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.QueryAllAsync(cancellationToken);
        return loans.Select(loan => _mapper.Map<LoanResponse>(loan)).ToList();
    }

    public async Task<LoanResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan is null ? null : _mapper.Map<LoanResponse>(loan);
    }

    public async Task<LoanResponse?> UpdateAsync(Guid id, UpdateLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        _mapper.Map(request, loan);
        loan.UpdatedOnUtc = _dateTimeProvider.UtcNow;
        await _repository.UpdateAsync(loan, cancellationToken);

        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task<LoanResponse?> UpdateStatusAsync(Guid id, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default)
    {
        await _statusValidator.ValidateAndThrowAsync(request, cancellationToken);

        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        loan.Status = request.Status;
        loan.UpdatedOnUtc = _dateTimeProvider.UtcNow;
        await _repository.UpdateAsync(loan, cancellationToken);

        return _mapper.Map<LoanResponse>(loan);
    }
}
