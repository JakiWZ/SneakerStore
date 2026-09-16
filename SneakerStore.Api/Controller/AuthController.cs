using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerStore.Api.Services;

namespace SneakerStore.Api.Controller;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateAdminRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// api/Auth
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/Auth/register — public sign-up, always creates a "User" account
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (success, error) = await _authService.Register(request.Username, request.Password, "User");
        if (!success)
            return BadRequest(new { error });
        return Ok();
    }

    // POST api/Auth/create-admin — only an existing Admin can create another Admin
    [HttpPost("create-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAdmin(CreateAdminRequest request)
    {
        var (success, error) = await _authService.Register(request.Username, request.Password, "Admin");
        if (!success)
            return BadRequest(new { error });
        return Ok();
    }

    // POST api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (success, token, role, error) = await _authService.Login(request.Username, request.Password);
        if (!success)
            return Unauthorized(new { error });
        return Ok(new { token, role, username = request.Username });
    }
}
