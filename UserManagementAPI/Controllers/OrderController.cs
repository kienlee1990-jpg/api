using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FastFoodAPI.DTOs.Order;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        // ===== checkout =====
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderId = await _service.CreateOrderAsync(userId, dto);

            return Ok(new { orderId });
        }

        // ===== my orders =====
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await _service.GetMyOrdersAsync(userId));
        }
    }
}