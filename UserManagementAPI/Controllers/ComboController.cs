using FastFoodAPI.DTOs.Combo;
using FastFoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        // ================= CREATE =================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ComboCreateDto dto)
        {
            var result = await _comboService.CreateAsync(dto);
            return Ok(new { message = "Tạo combo thành công", result });
        }

        // ================= GET PAGED =================
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] ComboQueryParams query)
        {
            var result = await _comboService.GetPagedAsync(query);
            return Ok(result);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var combo = await _comboService.GetByIdAsync(id);

            if (combo == null)
                return NotFound(new { message = "Không tìm thấy combo" });

            return Ok(combo);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ComboCreateDto dto)
        {
            var result = await _comboService.UpdateAsync(id, dto);

            if (!result)
                return NotFound(new { message = "Không tìm thấy combo để cập nhật" });

            return Ok(new { message = "Cập nhật combo thành công" });
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _comboService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy combo để xóa" });

            return Ok(new { message = "Xóa combo thành công" });
        }
    }
}