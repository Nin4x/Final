using AutoMapper;
using LoanApi.Application.DTOs.Auth;
using LoanApi.Application.Exceptions;
using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;

namespace LoanApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username) ?? await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new AppException("User with same username or email already exists", 409);
        }

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.Hash(request.Password);
        user.Role = Roles.User;

        await _userRepository.AddAsync(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = _tokenService.CreateToken(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.UsernameOrEmail) ?? await _userRepository.GetByEmailAsync(request.UsernameOrEmail);
        if (user == null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new AppException("Invalid credentials", 401);
        }

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = _tokenService.CreateToken(user)
        };
    }
}
