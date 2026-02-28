using FastFoodAPI.DTOs.User;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;
using FastFoodAPI.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // 🔵 ADMIN — GET ALL USERS
    public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email!,
                UserName = u.UserName!,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<UserDto>>.Ok(users);
    }

    // 🟡 UPDATE USER
    public async Task<ApiResponse<string>> UpdateUserAsync(
        string id,
        string currentUserId,
        bool isAdmin,
        UpdateUserDto dto)
    {
        if (!isAdmin && currentUserId != id)
            return ApiResponse<string>.Fail("You cannot update this user");

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        if (dto.FullName == null && dto.PhoneNumber == null)
            return ApiResponse<string>.Fail("No data to update");

        user.FullName = dto.FullName ?? user.FullName;
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return ApiResponse<string>.Fail(
                string.Join(",", result.Errors.Select(x => x.Description))
            );
        }

        return ApiResponse<string>.Ok("Profile updated successfully");
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            CreatedAt = user.CreatedAt
        };
    }
}