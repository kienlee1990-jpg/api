using FastFoodAPI.DTOs.Food;

namespace FastFoodAPI.Interfaces
{
    public interface IFoodService
    {
        Task<object> GetPagedAsync(FoodQueryParams query);
        Task<FoodResponseDto?> GetByIdAsync(int id);
        Task<FoodResponseDto> CreateAsync(FoodCreateDto dto);
        Task<bool> UpdateAsync(int id, FoodUpdateDto dto);
        Task<bool> PatchAsync(int id, FoodPatchDto dto);
        Task<bool> DeleteAsync(int id);
    }
}