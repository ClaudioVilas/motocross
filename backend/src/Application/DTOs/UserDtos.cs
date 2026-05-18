namespace Motocross.Application.DTOs;

public record UserRegistrationDto(
    string Email,
    string Password,
    string DisplayName);

public record UserLoginDto(
    string Email,
    string Password);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    DateTime CreatedAt);
