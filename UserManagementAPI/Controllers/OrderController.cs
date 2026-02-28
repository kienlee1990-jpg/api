using FastFoodAPI.DTOs.Order;
using FastFoodAPI.Interfaces;
using FastFoodAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    // ✅ POST create
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var id = await _service.CreateOrderAsync(userId, dto);

        return CreatedAtAction(nameof(GetById),
            new { id },
            ApiResponse<int>.Ok(id));
    }

    // ✅ GET by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var order = await _service.GetByIdAsync(id, userId);
        if (order == null) return NotFound();

        return Ok(ApiResponse<OrderDto>.Ok(order));
    }

    // ✅ GET my orders (pagination)
    [HttpGet("me")]
    public async Task<IActionResult> MyOrders(int page = 1, int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var result = await _service.GetMyOrdersAsync(userId, page, pageSize);

        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    // ✅ POST place from cart
    [HttpPost("place-from-cart")]
    public async Task<IActionResult> PlaceFromCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var id = await _service.PlaceOrderFromCartAsync(userId);

        return Ok(ApiResponse<int>.Ok(id));
    }
}