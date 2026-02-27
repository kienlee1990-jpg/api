using FastFoodAPI.DTOs.Food;

namespace FastFoodAPI.Interfaces
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodResponseDto>> GetAllAsync();
        Task<bool> CreateAsync(FoodCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}