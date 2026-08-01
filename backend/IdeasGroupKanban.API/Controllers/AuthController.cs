using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdeasGroupKanban.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        if (response == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(response);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }
}
