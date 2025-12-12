using AutoMapper;
using LoanApi.Application.DTOs.Users;
using LoanApi.Application.Exceptions;
using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;

namespace LoanApi.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse?> GetAsync(Guid id, Guid requesterId, string requesterRole)
    {
        if (requesterRole != Roles.Accountant && id != requesterId)
        {
            throw new ForbiddenException("Users may only view their own profile.");
        }

        var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
        return _mapper.Map<UserResponse>(user);
    }
}
