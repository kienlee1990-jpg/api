using Microsoft.EntityFrameworkCore;
using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Category;
using FastFoodAPI.Entities;
using FastFoodAPI.Interfaces;

namespace FastFoodAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(x => new CategoryResponseDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);

            var result = await _context.SaveChangesAsync();

            Console.WriteLine($"Saved rows = {result}");
            Console.WriteLine($"DB = {_context.Database.GetConnectionString()}");

            return result > 0;
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}