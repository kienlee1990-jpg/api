using FastFoodAPI.DTOs;

public interface ICartService
{
    Task<bool> AddToCartAsync(string userId, int? foodId, int? comboId, int quantity);

    Task<CartDto?> GetCartAsync(string userId);

    Task<bool> RemoveFromCartAsync(string userId, int? foodId, int? comboId);

    Task<bool> UpdateQuantityAsync(string userId, int? foodId, int? comboId, int quantity);
}