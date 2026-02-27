using FastFoodAPI.Data;
using FastFoodAPI.DTOs;
using FastFoodAPI.DTOs.Auth;
using FastFoodAPI.Entities;
using FastFoodAPI.Helper;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastFoodAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _config = config;
        _context = context;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.UserName,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));

        await _userManager.AddToRoleAsync(user, "Customer");

        // 🔥 tạo token luôn sau khi register (best practice)
        var accessToken = await GenerateJwtAsync(user);

        var refreshToken = new RefreshToken
        {
            Token = TokenHelper.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            Expiration = DateTime.UtcNow.AddHours(3)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new Exception("Invalid email or password");

        var accessToken = await GenerateJwtAsync(user);

        var refreshToken = new RefreshToken
        {
            Token = TokenHelper.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            Expiration = DateTime.UtcNow.AddHours(3)
        };
    }

    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? "")
    };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"], // ✅ FIX TẠI ĐÂY
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);

        if (refreshToken == null ||
            refreshToken.IsRevoked ||
            refreshToken.Expires < DateTime.UtcNow)
            throw new Exception("Invalid refresh token");

        // revoke token cũ
        refreshToken.IsRevoked = true;

        var newRefreshToken = new RefreshToken
        {
            Token = TokenHelper.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = refreshToken.UserId,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshToken);

        var accessToken = await GenerateJwtAsync(refreshToken.User!);

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            Expiration = DateTime.UtcNow.AddHours(3)
        };
    }

    public async Task LogoutAsync(ClaimsPrincipal userPrincipal, string refreshToken)
    {
        // 🔹 lấy userId từ JWT
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid token");

        // 🔹 tìm refresh token
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null)
            throw new Exception("Refresh token not found");

        // 🔥 CHECK QUAN TRỌNG — token có thuộc user không
        if (token.UserId != userId)
            throw new UnauthorizedAccessException("You cannot logout this token");

        // 🔹 revoke token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RequestPasswordResetAsync(RequestPasswordResetDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // bảo mật: luôn trả OK
        if (user == null)
            return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        var resetLink =
            $"https://yourdomain.com/reset-password?email={dto.Email}&token={encodedToken}";

        // TODO: gửi email thật
        Console.WriteLine($"RESET LINK: {resetLink}");
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Invalid request");

        var decodedToken = Uri.UnescapeDataString(dto.Token);

        var result = await _userManager.ResetPasswordAsync(
            user,
            decodedToken,
            dto.NewPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
    }
}