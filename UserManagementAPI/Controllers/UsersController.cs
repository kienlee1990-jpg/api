using FastFoodAPI.DTOs.User;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // 🔵 ADMIN ONLY
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    // 🟡 ADMIN or OWNER
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var result = await _userService.UpdateUserAsync(
            id,
            currentUserId,
            isAdmin,
            dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // 🟢 CURRENT USER PROFILE
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _userService.GetUserByIdAsync(currentUserId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}