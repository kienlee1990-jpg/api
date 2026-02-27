using Microsoft.EntityFrameworkCore;
using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Food;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;

        public FoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodResponseDto>> GetAllAsync()
        {
            return await _context.Foods
                .Include(x => x.Category)
                .Select(x => new FoodResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    CategoryName = x.Category.Name
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(FoodCreateDto dto)
        {
            var food = new Food
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl
            };

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}