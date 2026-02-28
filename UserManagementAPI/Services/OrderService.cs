using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Order;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;
using FastFoodAPI.Responses;
using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= CREATE ORDER =================
    public async Task<int> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than 0");

                // ================= FOOD =================
                if (item.FoodId.HasValue)
                {
                    var food = await _context.Foods.FindAsync(item.FoodId.Value);

                    if (food == null)
                        throw new Exception("Food not found");

                    // 🔴 NEW: check available
                    if (!food.IsAvailable)
                        throw new Exception($"{food.Name} is not available");

                    if (food.StockQuantity < item.Quantity)
                        throw new Exception($"{food.Name} out of stock");

                    // ✅ trừ tồn food
                    food.StockQuantity -= item.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        FoodId = food.Id,
                        Quantity = item.Quantity,
                        Price = food.Price
                    });

                    total += food.Price * item.Quantity;
                }

                // ================= COMBO =================
                else if (item.ComboId.HasValue)
                {
                    var combo = await _context.Combos
                        .Include(c => c.ComboFoods)
                        .ThenInclude(cf => cf.Food)
                        .FirstOrDefaultAsync(c => c.Id == item.ComboId.Value);

                    if (combo == null)
                        throw new Exception("Combo not found");

                    // 🔴 NEW: check available
                    if (!combo.IsAvailable)
                        throw new Exception($"{combo.Name} is not available");

                    // 🔴 check tồn combo
                    if (combo.StockQuantity < item.Quantity)
                        throw new Exception($"{combo.Name} out of stock");

                    // 🔴 check tồn từng food trong combo
                    foreach (var cf in combo.ComboFoods)
                    {
                        var required = cf.Quantity * item.Quantity;

                        if (cf.Food.StockQuantity < required)
                            throw new Exception($"{cf.Food.Name} out of stock for combo");
                    }

                    // ✅ trừ tồn từng food
                    foreach (var cf in combo.ComboFoods)
                    {
                        var required = cf.Quantity * item.Quantity;
                        cf.Food.StockQuantity -= required;
                    }

                    // ✅ trừ tồn combo
                    combo.StockQuantity -= item.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        ComboId = combo.Id,
                        Quantity = item.Quantity,
                        Price = combo.Price
                    });

                    total += combo.Price * item.Quantity;
                }
                else
                {
                    throw new Exception("FoodId or ComboId is required");
                }
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
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

    // ================= GET BY ID =================
    public async Task<OrderDto?> GetByIdAsync(int id, string userId)
    {
        var order = await _context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (order == null) return null;

        return new OrderDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };
    }

    // ================= GET MY ORDERS =================
    public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(
        string userId, int page, int pageSize)
    {
        var query = _context.Orders
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new OrderDto
            {
                Id = x.Id,
                TotalAmount = x.TotalAmount,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<OrderDto>(items, total, page, pageSize);
    }

    // ================= PLACE ORDER FROM CART =================
    public async Task<int> PlaceOrderFromCartAsync(string userId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 🔥 lấy cart items đúng theo user
            var cartItems = await _context.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            // 🔥 load foods 1 lần (tối ưu)
            var foodIds = cartItems.Select(x => x.FoodId).ToList();

            var foods = await _context.Foods
                .Where(f => foodIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id);

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var ci in cartItems)
            {
                if (ci.FoodId is not int foodId ||
    !foods.TryGetValue(foodId, out var food))
                {
                    throw new Exception("Food not found");
                }
                // 🔴 check tồn kho
                if (food.StockQuantity < ci.Quantity)
                    throw new Exception($"{food.Name} out of stock");

                // 🔴 trừ kho
                food.StockQuantity -= ci.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    FoodId = food.Id,
                    Quantity = ci.Quantity,
                    Price = food.Price
                });

                total += food.Price * ci.Quantity;
            }

            order.TotalAmount = total;

            // 🔥 clear cart
            _context.CartItems.RemoveRange(cartItems);

            // 🔥 add order
            _context.Orders.Add(order);

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