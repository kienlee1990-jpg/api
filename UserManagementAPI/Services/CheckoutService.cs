using FastFoodAPI.Data;
using FastFoodAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services
{
    public class CheckoutService
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

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. Lấy cart của user
                // =====================================================
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Food)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    throw new Exception("Cart is empty");

                // =====================================================
                // 2. Tạo Order
                // =====================================================
                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    TotalPrice = 0
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // =====================================================
                // 3. Tạo OrderItems + tính tổng
                // =====================================================
                decimal total = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in cart.CartItems)
                {
                    if (item.Food == null)
                        throw new Exception($"Food not found (FoodId={item.FoodId})");

                    var price = item.Food.Price;
                    var lineTotal = item.Quantity * price;
                    total += lineTotal;

                    orderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Price = price
                    });
                }

                await _context.OrderItems.AddRangeAsync(orderItems);

                // =====================================================
                // 4. Update tổng tiền
                // =====================================================
                order.TotalPrice = total;
                _context.Orders.Update(order);

                // =====================================================
                // 5. Clear cart
                // =====================================================
                _context.CartItems.RemoveRange(cart.CartItems);

                // =====================================================
                // 6. Save + Commit
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
}