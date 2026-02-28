using FastFoodAPI.DTOs.Combo;

public interface IComboService
{
    Task<bool> CreateAsync(ComboCreateDto dto);
    Task<object> GetPagedAsync(ComboQueryParams query);
    Task<ComboResponseDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, ComboCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<int> CalculateComboStockAsync(int comboId);
    Task UpdateCombosByFoodSqlAsync(int foodId);
}