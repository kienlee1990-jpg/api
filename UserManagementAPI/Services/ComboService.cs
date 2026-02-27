using Microsoft.EntityFrameworkCore;
using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Combo;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Services
{
    public class ComboService : IComboService
    {
        private readonly ApplicationDbContext _context;

        public ComboService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(ComboCreateDto dto)
        {
            var combo = new Combo
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description
            };

            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            // thêm food vào combo
            var comboFoods = dto.Foods.Select(x => new ComboFood
            {
                ComboId = combo.Id,
                FoodId = x.FoodId,
                Quantity = x.Quantity
            });

            _context.ComboFoods.AddRange(comboFoods);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ComboResponseDto>> GetAllAsync()
        {
            return await _context.Combos
                .Include(c => c.ComboFoods)
                .ThenInclude(cf => cf.Food)
                .Select(c => new ComboResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    Foods = c.ComboFoods.Select(x => x.Food.Name).ToList()
                })
                .ToListAsync();
        }
    }
}