using FastFoodAPI.DTOs.Combo;

namespace FastFoodAPI.Interfaces
{
    public interface IComboService
    {
        Task<bool> CreateAsync(ComboCreateDto dto);
        Task<IEnumerable<ComboResponseDto>> GetAllAsync();
    }
}