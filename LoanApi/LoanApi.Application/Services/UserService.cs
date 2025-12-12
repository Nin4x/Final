using AutoMapper;
using LoanApi.Application.DTOs;
using LoanApi.Application.Exceptions;
using LoanApi.Application.Interfaces;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default)
    {
        EnsureAccountantOrOwner(id, currentUserId, currentUserRole, "Users can only view their own profile.");

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> SetBlockStatusAsync(Guid id, bool isBlocked, Guid currentUserId, UserRole currentUserRole, CancellationToken cancellationToken = default)
    {
        EnsureAccountant(currentUserRole, "Only accountants can update user block status.");

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.IsBlocked = isBlocked;
        user.UpdatedOnUtc = _dateTimeProvider.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    private static void EnsureAccountant(UserRole currentUserRole, string message)
    {
        if (currentUserRole != UserRole.Accountant)
        {
            throw new ForbiddenException(message);
        }
    }

    private static void EnsureAccountantOrOwner(Guid targetUserId, Guid currentUserId, UserRole currentUserRole, string message)
    {
        if (currentUserRole != UserRole.Accountant && targetUserId != currentUserId)
        {
            throw new ForbiddenException(message);
        }
    }
}
