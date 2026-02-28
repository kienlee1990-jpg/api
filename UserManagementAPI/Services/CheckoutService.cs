using FastFoodAPI.Data;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ApplicationDbContext _context;

    public CheckoutService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CheckoutAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Invalid user");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // =====================================================
            // 1. Load cart + include sâu
            // =====================================================
            var cart = await _context.Carts
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo)
                        .ThenInclude(co => co.ComboFoods!)
                            .ThenInclude(cf => cf.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems!.Any())
                throw new Exception("Cart is empty");

            // =====================================================
            // 2. Create order
            // =====================================================
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // =====================================================
            // 3. Build order items + trừ kho
            // =====================================================
            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Invalid quantity");

                decimal price;

                // ================= FOOD =================
                if (item.FoodId.HasValue)
                {
                    var food = item.Food
                        ?? throw new Exception($"Food not found (FoodId={item.FoodId})");

                    if (!food.IsAvailable)
                        throw new Exception($"Food '{food.Name}' is unavailable");

                    if (food.StockQuantity < item.Quantity)
                        throw new Exception($"Food '{food.Name}' out of stock");

                    price = food.Price;

                    // 🔥 trừ kho food
                    food.StockQuantity -= item.Quantity;
                }
                // ================= COMBO =================
                else if (item.ComboId.HasValue)
                {
                    var combo = item.Combo
                        ?? throw new Exception($"Combo not found (ComboId={item.ComboId})");

                    if (!combo.IsAvailable)
                        throw new Exception($"Combo '{combo.Name}' is unavailable");

                    if (combo.StockQuantity < item.Quantity)
                        throw new Exception($"Combo '{combo.Name}' out of stock");

                    price = combo.Price;

                    // 🔥 trừ kho combo (nếu bạn quản lý)
                    combo.StockQuantity -= item.Quantity;

                    // =====================================================
                    // 🔥 QUAN TRỌNG: trừ tồn từng FOOD trong combo
                    // =====================================================
                    if (combo.ComboFoods == null || !combo.ComboFoods.Any())
                        throw new Exception($"Combo '{combo.Name}' has no foods");

                    foreach (var comboFood in combo.ComboFoods)
                    {
                        var food = comboFood.Food
                            ?? throw new Exception("Combo food not found");

                        var requiredQty = comboFood.Quantity * item.Quantity;

                        if (food.StockQuantity < requiredQty)
                            throw new Exception(
                                $"Food '{food.Name}' is not enough for combo");

                        // ✅ trừ kho food trong combo
                        food.StockQuantity -= requiredQty;
                    }
                }
                else
                {
                    throw new Exception("Invalid cart item");
                }

                var lineTotal = price * item.Quantity;
                total += lineTotal;

                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    FoodId = item.FoodId,
                    ComboId = item.ComboId,
                    Quantity = item.Quantity,
                    Price = price
                });
            }

            await _context.OrderItems.AddRangeAsync(orderItems);

            // =====================================================
            // 4. Update total
            // =====================================================
            order.TotalAmount = total;

            // =====================================================
            // 5. Clear cart
            // =====================================================
            _context.CartItems.RemoveRange(cart.CartItems);

            // =====================================================
            // 6. Save + commit
            // =====================================================
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}