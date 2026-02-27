using FastFoodAPI.DTOs.Category;

namespace FastFoodAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}