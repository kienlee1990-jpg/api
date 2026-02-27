using FastFoodAPI.DTOs;
using FastFoodAPI.DTOs.Auth;
using System.Security.Claims;

namespace FastFoodAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task RequestPasswordResetAsync(RequestPasswordResetDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(ClaimsPrincipal user, string refreshToken);
}