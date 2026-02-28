using FastFoodAPI.DTOs.Food;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // ================= GET PAGED =================
        // GET: api/food?pageNumber=1&pageSize=10&search=ga&categoryId=2
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FoodQueryParams query)
        {
            var result = await _service.GetPagedAsync(query);
            return Ok(result);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var food = await _service.GetByIdAsync(id);
            if (food == null) return NotFound();
            return Ok(food);
        }

        // ================= CREATE =================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] FoodCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = ((dynamic)created).Id },
                created);
        }

        // ================= PUT =================
        // Update toàn bộ
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] FoodUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result) return NotFound();

            return Ok(new { message = "Updated successfully" });
        }

        // ================= PATCH =================
        // Update một phần
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Patch(int id, [FromBody] FoodPatchDto dto)
        {
            var result = await _service.PatchAsync(id, dto);
            if (!result) return NotFound();

            return Ok(new { message = "Patched successfully" });
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}