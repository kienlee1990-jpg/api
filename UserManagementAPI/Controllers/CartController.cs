using FastFoodAPI.Interfaces;
using FastFoodAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        // ⭐ GET CART
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // =====================================================
        // ⭐ ADD TO CART
        // =====================================================
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int foodId, int quantity = 1)
        {
            var userId = GetUserId();

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            await _cartService.AddToCartAsync(userId, foodId, quantity);

            return Ok(new { message = "Added to cart" });
        }

        // =====================================================
        // ⭐ UPDATE QUANTITY
        // =====================================================
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity(int foodId, int quantity)
        {
            var userId = GetUserId();

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            await _cartService.UpdateQuantityAsync(userId, foodId, quantity);

            return Ok(new { message = "Cart updated" });
        }

        // =====================================================
        // ⭐ REMOVE ITEM
        // =====================================================
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveItem(int foodId)
        {
            var userId = GetUserId();

            await _cartService.RemoveFromCartAsync(userId, foodId);

            return Ok(new { message = "Item removed" });
        }

        // =====================================================
        // ⭐ CHECKOUT
        // =====================================================
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();

            try
            {
                var orderId = await _checkoutService.CheckoutAsync(userId);

                return Ok(new
                {
                    message = "Checkout success",
                    orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}