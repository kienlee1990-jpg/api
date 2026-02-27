using FastFoodAPI.DTOs.Order;

namespace FastFoodAPI.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(string userId);
    }
}