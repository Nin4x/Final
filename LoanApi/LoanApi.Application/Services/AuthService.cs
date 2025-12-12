using AutoMapper;
using FluentValidation;
using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace LoanApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher<User> passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _mapper = mapper;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            throw new ValidationException("Username or email is already in use.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            Role = request.Role,
            CreatedOnUtc = _dateTimeProvider.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.CreateAsync(user, cancellationToken);

        var tokenResult = _jwtTokenGenerator.GenerateToken(user);
        var userResponse = _mapper.Map<UserResponse>(user);

        return new AuthResponse(tokenResult.AccessToken, tokenResult.ExpiresOnUtc, userResponse);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByUsernameAsync(request.UsernameOrEmail, cancellationToken)
            ?? await _userRepository.GetByEmailAsync(request.UsernameOrEmail, cancellationToken);
        if (user is null)
        {
            throw new ValidationException("Invalid credentials.");
        }

        var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            throw new ValidationException("Invalid credentials.");
        }

        var tokenResult = _jwtTokenGenerator.GenerateToken(user);
        var userResponse = _mapper.Map<UserResponse>(user);

        return new AuthResponse(tokenResult.AccessToken, tokenResult.ExpiresOnUtc, userResponse);
    }
}
