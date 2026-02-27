using Microsoft.EntityFrameworkCore;
using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Order;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Services
{
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
            // lấy food từ DB
            var foodIds = dto.Items.Select(x => x.FoodId).ToList();

            var foods = await _context.Foods
                .Where(x => foodIds.Contains(x.Id))
                .ToListAsync();

            decimal total = 0;

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var food = foods.First(x => x.Id == item.FoodId);

                var price = food.Price * item.Quantity;
                total += price;

                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    FoodId = item.FoodId,
                    Quantity = item.Quantity,
                    Price = food.Price
                });
            }

            order.TotalPrice = total;

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        // ================= GET MY ORDERS =================
        public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(i => new OrderItemDto
                    {
                        FoodName = i.Food.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}