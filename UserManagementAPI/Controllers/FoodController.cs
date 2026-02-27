using Microsoft.AspNetCore.Mvc;
using FastFoodAPI.DTOs.Food;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _service;

        public FoodController(IFoodService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create(FoodCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Food created");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok("Deleted");
        }
    }
}