using Microsoft.EntityFrameworkCore;
using FastFoodAPI.Data;
using FastFoodAPI.Entities;
using FastFoodAPI.DTOs;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 🟢 ADD TO CART
        // =========================================================
        public async Task<bool> AddToCartAsync(string userId, int foodId, int quantity)
        {
            if (quantity <= 0) return false;

            var food = await _context.Foods.FindAsync(foodId);
            if (food == null) return false;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems?
                .FirstOrDefault(i => i.FoodId == foodId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems!.Add(new CartItem
                {
                    FoodId = foodId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================================================
        // 🟢 GET CART
        // =========================================================
        public async Task<CartDto?> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return null;

            var dto = new CartDto
            {
                Id = cart.Id,
                Items = cart.CartItems?.Select(ci => new CartItemDto
                {
                    FoodId = ci.FoodId,
                    FoodName = ci.Food.Name,
                    Price = ci.Food.Price,
                    Quantity = ci.Quantity,
                    SubTotal = ci.Quantity * ci.Food.Price
                }).ToList() ?? new List<CartItemDto>()
            };

            dto.TotalPrice = dto.Items.Sum(i => i.SubTotal);

            return dto;
        }

        // =========================================================
        // 🟢 REMOVE FROM CART
        // =========================================================
        public async Task<bool> RemoveFromCartAsync(string userId, int foodId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var item = cart.CartItems?
                .FirstOrDefault(i => i.FoodId == foodId);

            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // 🟢 UPDATE QUANTITY
        // =========================================================
        public async Task<bool> UpdateQuantityAsync(string userId, int foodId, int quantity)
        {
            if (quantity <= 0) return false;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var item = cart.CartItems?
                .FirstOrDefault(i => i.FoodId == foodId);

            if (item == null) return false;

            item.Quantity = quantity;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}