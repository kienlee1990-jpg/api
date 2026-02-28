using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodAPI.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    // =====================================================
    // 🔑 helper lấy userId từ JWT (chuẩn Identity)
    // =====================================================
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User not found in token");

        return userId;
    }

    // =====================================================
    // ⭐ POST /api/checkout
    // =====================================================
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var userId = GetUserId();

            var orderId = await _checkoutService.CheckoutAsync(userId);

            return Ok(new
            {
                message = "Checkout successful",
                orderId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}