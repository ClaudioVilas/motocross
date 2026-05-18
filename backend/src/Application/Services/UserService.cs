using Microsoft.AspNetCore.Identity;
using Motocross.Application.DTOs;
using Motocross.Application.Interfaces;
using Motocross.Domain.Entities;

namespace Motocross.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher<UserAccount> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> RegisterAsync(UserRegistrationDto registration, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistsByEmailAsync(registration.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new UserAccount(
            registration.Email,
            registration.DisplayName);

        user.SetPasswordHash(_passwordHasher.HashPassword(user, registration.Password));

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto(user.Id, user.Email, user.DisplayName, user.CreatedAt);
    }

    public async Task<UserDto> LoginAsync(UserLoginDto login, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(login.Email, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        return new UserDto(user.Id, user.Email, user.DisplayName, user.CreatedAt);
    }
}
