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
        public async Task<bool> AddToCartAsync(
            string userId,
            int? foodId,
            int? comboId,
            int quantity)
        {
            if (quantity <= 0) return false;

            // ❌ phải chọn đúng 1 loại
            if ((foodId == null && comboId == null) ||
                (foodId != null && comboId != null))
                return false;

            int stockQuantity;

            // ================= FOOD =================
            if (foodId.HasValue)
            {
                var food = await _context.Foods.FindAsync(foodId.Value);
                if (food == null || !food.IsAvailable) return false;
                if (food.StockQuantity <= 0) return false;

                stockQuantity = food.StockQuantity;
            }
            else
            {
                var combo = await _context.Combos.FindAsync(comboId!.Value);
                if (combo == null || !combo.IsAvailable) return false;
                if (combo.StockQuantity <= 0) return false;

                stockQuantity = combo.StockQuantity;
            }

            // ================= GET CART =================
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

            // ================= FIND EXISTING =================
            var existingItem = cart.CartItems!.FirstOrDefault(i =>
                i.FoodId == foodId &&
                i.ComboId == comboId);

            var newQuantity = (existingItem?.Quantity ?? 0) + quantity;

            // 🔴 CHECK STOCK
            if (newQuantity > stockQuantity)
                return false;

            // ================= UPDATE =================
            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
            }
            else
            {
                cart.CartItems!.Add(new CartItem
                {
                    FoodId = foodId,
                    ComboId = comboId,
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
                .AsNoTracking()
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return null;

            var dto = new CartDto
            {
                Id = cart.Id,
                Items = cart.CartItems?.Select(ci =>
                {
                    var price = ci.Food != null
                        ? ci.Food.Price
                        : ci.Combo!.Price;

                    return new CartItemDto
                    {
                        FoodId = ci.FoodId,
                        ComboId = ci.ComboId,
                        FoodName = ci.Food?.Name,
                        ComboName = ci.Combo?.Name,
                        Price = price,
                        Quantity = ci.Quantity,
                        SubTotal = price * ci.Quantity
                    };
                }).ToList() ?? new List<CartItemDto>()
            };

            dto.TotalPrice = dto.Items.Sum(i => i.SubTotal);
            return dto;
        }

        // =========================================================
        // 🟢 REMOVE ITEM
        // =========================================================
        public async Task<bool> RemoveFromCartAsync(
            string userId,
            int? foodId,
            int? comboId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems!)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var item = cart.CartItems!.FirstOrDefault(i =>
                i.FoodId == foodId &&
                i.ComboId == comboId);

            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        // =========================================================
        // 🟢 UPDATE QUANTITY
        // =========================================================
        public async Task<bool> UpdateQuantityAsync(
            string userId,
            int? foodId,
            int? comboId,
            int quantity)
        {
            if (quantity <= 0) return false;

            var cart = await _context.Carts
                .Include(c => c.CartItems!)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var item = cart.CartItems!.FirstOrDefault(i =>
                i.FoodId == foodId &&
                i.ComboId == comboId);

            if (item == null) return false;

            item.Quantity = quantity;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}