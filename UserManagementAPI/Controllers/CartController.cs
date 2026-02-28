using FastFoodAPI.Interfaces;
using FastFoodAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FastFoodAPI.DTOs.Cart;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly CheckoutService _checkoutService;

        public CartController(
            ICartService cartService,
            CheckoutService checkoutService)
        {
            _cartService = cartService;
            _checkoutService = checkoutService;
        }

        // =====================================================
        // ⭐ helper lấy userId từ JWT
        // =====================================================
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not found in token");

            return userId;
        }

        // =====================================================
        // ⭐ GET MY CART
        // GET /api/cart
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
                return Ok(new { message = "Cart is empty" });

            return Ok(cart);
        }

        // =====================================================
        // ⭐ ADD ITEM
        // POST /api/cart/items
        // =====================================================
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            var result = await _cartService.AddToCartAsync(
                userId,
                dto.FoodId,
                dto.ComboId,
                dto.Quantity);

            if (!result)
                return BadRequest("Cannot add item to cart");

            return Ok(new { message = "Added to cart" });
        }

        // =====================================================
        // ⭐ UPDATE QUANTITY
        // PUT /api/cart/items
        // =====================================================
        [HttpPut("items")]
        public async Task<IActionResult> UpdateQuantity(
            [FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            var result = await _cartService.UpdateQuantityAsync(
                userId,
                dto.FoodId,
                dto.ComboId,
                dto.Quantity);

            if (!result)
                return NotFound("Item not found in cart");

            return Ok(new { message = "Cart updated" });
        }

        // =====================================================
        // ⭐ REMOVE ITEM
        // DELETE /api/cart/items
        // =====================================================
        [HttpDelete("items")]
        public async Task<IActionResult> RemoveItem(
            [FromQuery] int? foodId,
            [FromQuery] int? comboId)
        {
            var userId = GetUserId();

            var result = await _cartService.RemoveFromCartAsync(
                userId,
                foodId,
                comboId);

            if (!result)
                return NotFound("Item not found in cart");

            return Ok(new { message = "Item removed" });
        }

        // =====================================================
        // ⭐ CHECKOUT
        // POST /api/cart/checkout
        // =====================================================
        
    }
}