using FastFoodAPI.DTOs;

namespace FastFoodAPI.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(string userId, int foodId, int quantity);
        Task<CartDto?> GetCartAsync(string userId);
        Task<bool> RemoveFromCartAsync(string userId, int foodId);
        Task<bool> UpdateQuantityAsync(string userId, int foodId, int quantity);
    }
}