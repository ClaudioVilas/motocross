using Motocross.Application.DTOs;

namespace Motocross.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(UserRegistrationDto registration, CancellationToken cancellationToken = default);
    Task<UserDto> LoginAsync(UserLoginDto login, CancellationToken cancellationToken = default);
}
