using Microsoft.AspNetCore.Mvc;
using FastFoodAPI.DTOs.Combo;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _service;

        public ComboController(IComboService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create(ComboCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Combo created");
        }
    }
}