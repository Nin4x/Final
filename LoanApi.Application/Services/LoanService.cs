using AutoMapper;
using LoanApi.Application.DTOs.Loans;
using LoanApi.Application.Exceptions;
using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public LoanService(ILoanRepository loanRepository, IUserRepository userRepository, IMapper mapper)
    {
        _loanRepository = loanRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request, Guid userId, string role, bool isBlocked)
    {
        if (isBlocked)
        {
            throw new ForbiddenException("User is blocked and cannot create loans");
        }

        var loan = _mapper.Map<Loan>(request);
        loan.UserId = userId;
        loan.Status = LoanStatus.Processing;
        loan.Id = Guid.NewGuid();

        await _loanRepository.AddAsync(loan);
        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task<IEnumerable<LoanResponse>> GetAsync(Guid requesterId, string role)
    {
        IEnumerable<Loan> loans = role == Roles.Accountant
            ? await _loanRepository.GetAllAsync()
            : await _loanRepository.GetByUserIdAsync(requesterId);

        return loans.Select(_mapper.Map<LoanResponse>);
    }

    public async Task<LoanResponse?> GetByIdAsync(Guid loanId, Guid requesterId, string role)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId) ?? throw new NotFoundException("Loan not found");
        if (role != Roles.Accountant && loan.UserId != requesterId)
        {
            throw new ForbiddenException("You cannot view this loan");
        }
        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task<LoanResponse> UpdateAsync(Guid loanId, UpdateLoanRequest request, Guid requesterId, string role)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId) ?? throw new NotFoundException("Loan not found");
        if (role != Roles.Accountant && loan.UserId != requesterId)
        {
            throw new ForbiddenException("You cannot update this loan");
        }
        if (role == Roles.User && loan.Status != LoanStatus.Processing)
        {
            throw new AppException("Loan can be updated only while processing", 400);
        }

        loan.LoanType = request.LoanType;
        loan.Amount = request.Amount;
        loan.Currency = request.Currency;
        loan.Period = request.Period;

        await _loanRepository.UpdateAsync(loan);
        return _mapper.Map<LoanResponse>(loan);
    }

    public async Task DeleteAsync(Guid loanId, Guid requesterId, string role)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId) ?? throw new NotFoundException("Loan not found");
        if (role != Roles.Accountant && loan.UserId != requesterId)
        {
            throw new ForbiddenException("You cannot delete this loan");
        }
        if (role == Roles.User && loan.Status != LoanStatus.Processing)
        {
            throw new AppException("Loan can be deleted only while processing", 400);
        }

        await _loanRepository.DeleteAsync(loan);
    }

    public async Task<LoanResponse> ChangeStatusAsync(Guid loanId, string status, string role)
    {
        if (role != Roles.Accountant)
        {
            throw new ForbiddenException("Only accountants can change status");
        }

        var loan = await _loanRepository.GetByIdAsync(loanId) ?? throw new NotFoundException("Loan not found");
        loan.Status = status.ToLower() switch
        {
            "approved" => LoanStatus.Approved,
            "rejected" => LoanStatus.Rejected,
            _ => throw new AppException("Status must be Approved or Rejected", 400)
        };

        await _loanRepository.UpdateAsync(loan);
        return _mapper.Map<LoanResponse>(loan);
    }
}
