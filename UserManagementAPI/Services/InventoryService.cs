using FastFoodAPI.Data;
using FastFoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IComboService _comboService; // ⭐ thêm

    public InventoryService(
        ApplicationDbContext context,
        IComboService comboService) // ⭐ inject
    {
        _context = context;
        _comboService = comboService;
    }

    public async Task StockInAsync(int foodId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than 0");

        var food = await _context.Foods.FindAsync(foodId);

        if (food == null)
            throw new Exception("Food not found");

        // ✅ tăng tồn food
        food.StockQuantity += quantity;

        // ✅ update combo theo SQL
        await _comboService.UpdateCombosByFoodSqlAsync(food.Id);

        await _context.SaveChangesAsync();
    }
}