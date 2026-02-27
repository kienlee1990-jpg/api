using FastFoodAPI.DTOs.Category;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var success = await _service.CreateAsync(dto);

            if (!success)
                return BadRequest("Create failed");

            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result) return NotFound();
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}