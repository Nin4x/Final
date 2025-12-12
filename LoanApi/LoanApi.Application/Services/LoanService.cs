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
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<CreateLoanRequest> _createValidator;
    private readonly IValidator<UpdateLoanRequest> _updateValidator;
    private readonly IValidator<UpdateLoanStatusRequest> _statusValidator;
    private readonly IMapper _mapper;

    public LoanService(
        ILoanRepository repository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IValidator<CreateLoanRequest> createValidator,
        IValidator<UpdateLoanRequest> updateValidator,
        IValidator<UpdateLoanStatusRequest> statusValidator,
        IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
        _mapper = mapper;
    }

    public async Task<LoanResponse> CreateAsync(Guid currentUserId, UserRole currentUserRole, CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new ValidationException("User not found.");

        if (currentUserRole == UserRole.User && user.IsBlocked)
        {
            throw new ValidationException("Blocked users cannot create loans.");
        }

        var loan = _mapper.Map<Loan>(request);
        loan.Id = Guid.NewGuid();
        loan.CreatedOnUtc = _dateTimeProvider.UtcNow;
        loan.UserId = currentUserId;

        var created = await _repository.CreateAsync(loan, cancellationToken);
        return _mapper.Map<LoanResponse>(created);
    }

    public async Task<IReadOnlyCollection<LoanResponse>> GetAllAsync(Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default)
    {
        var loans = currentUserRole == UserRole.Accountant
            ? await _repository.QueryAllAsync(cancellationToken)
            : await _repository.GetByUserIdAsync(currentUserId, cancellationToken);
        return loans.Select(loan => _mapper.Map<LoanResponse>(loan)).ToList();
    }

    public async Task<LoanResponse?> GetAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        if (currentUserRole != UserRole.Accountant && loan.UserId != currentUserId)
        {
            return null;
        }

        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task<LoanResponse?> UpdateAsync(Guid id, Guid currentUserId, UserRole currentUserRole, UpdateLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return null;
        }

        if (currentUserRole != UserRole.Accountant)
        {
            if (loan.UserId != currentUserId)
            {
                return null;
            }

            if (loan.Status != LoanStatus.Processing)
            {
                throw new ValidationException("Only processing loans can be updated.");
            }
        }

        _mapper.Map(request, loan);
        loan.UpdatedOnUtc = _dateTimeProvider.UtcNow;
        await _repository.UpdateAsync(loan, cancellationToken);

        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task<LoanResponse?> UpdateStatusAsync(Guid id, Guid currentUserId, UserRole currentUserRole, UpdateLoanStatusRequest request, CancellationToken cancellationToken = default)
    {
        await _statusValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (currentUserRole != UserRole.Accountant)
        {
            throw new ValidationException("Only accountants can update loan status.");
        }

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

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
        {
            return false;
        }

        if (currentUserRole != UserRole.Accountant)
        {
            if (loan.UserId != currentUserId)
            {
                return false;
            }

            if (loan.Status != LoanStatus.Processing)
            {
                throw new ValidationException("Only processing loans can be deleted by the owner.");
            }
        }

        await _repository.DeleteAsync(loan, cancellationToken);
        return true;
    }
}
