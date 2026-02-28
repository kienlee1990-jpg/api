using FastFoodAPI.DTOs.Order;
using FastFoodAPI.Responses;

namespace FastFoodAPI.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<OrderDto?> GetByIdAsync(int id, string userId);
        Task<PagedResult<OrderDto>> GetMyOrdersAsync(string userId, int page, int pageSize);
        Task<int> PlaceOrderFromCartAsync(string userId);
    }
}