using FastFoodAPI.DTOs;
using FastFoodAPI.DTOs.Auth;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
        => Ok(await _authService.LoginAsync(dto));

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        => Ok(await _authService.RefreshTokenAsync(dto.RefreshToken));

    [Authorize]
    [HttpDelete("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await _authService.LogoutAsync(User, dto.RefreshToken);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto dto)
    {
        await _authService.RequestPasswordResetAsync(dto);
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return NoContent();
    }
}