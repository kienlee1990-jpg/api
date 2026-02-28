using FastFoodAPI.DTOs.User;
using FastFoodAPI.Responses;

namespace FastFoodAPI.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<string>> UpdateUserAsync(string id, string currentUserId, bool isAdmin, UpdateUserDto dto);
    Task<UserDto?> GetUserByIdAsync(string id);
}