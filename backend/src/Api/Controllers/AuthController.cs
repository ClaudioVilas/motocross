using Microsoft.AspNetCore.Mvc;
using Motocross.Application.DTOs;
using Motocross.Application.Interfaces;

namespace Motocross.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] UserRegistrationDto registration)
    {
        try
        {
            var user = await _userService.RegisterAsync(registration);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for {Email}", registration.Email);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto login)
    {
        try
        {
            var user = await _userService.LoginAsync(login);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Login failed for {Email}", login.Email);
            return Unauthorized(new { message = ex.Message });
        }
    }
}
