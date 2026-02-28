using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Food;
using FastFoodAPI.Entities;
using FastFoodAPI.Helpers;
using FastFoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;

        public FoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= GET PAGED =================
        public async Task<object> GetPagedAsync(FoodQueryParams query)
        {
            var foodsQuery = _context.Foods
                .Include(x => x.Category)
                .AsQueryable();

            // ================= FILTER IsAvailable =================
            if (query.IsAvailable.HasValue)
            {
                foodsQuery = foodsQuery.Where(x => x.IsAvailable == query.IsAvailable.Value);
            }

            // ================= FILTER Category =================
            if (query.CategoryId.HasValue)
            {
                foodsQuery = foodsQuery.Where(x => x.CategoryId == query.CategoryId.Value);
            }

            // 🔥 load về RAM để search không dấu
            var foodsList = await foodsQuery.ToListAsync();

            // ================= SEARCH đa trường =================
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = StringHelper.RemoveDiacritics(query.Search).ToLower();

                foodsList = foodsList
                    .Where(x =>
                        StringHelper.RemoveDiacritics(x.Name).ToLower().Contains(keyword)
                        || StringHelper.RemoveDiacritics(x.Category.Name).ToLower().Contains(keyword)
                    )
                    .ToList();
            }

            // ================= FILTER giá gần đúng =================
            if (query.Price.HasValue)
            {
                var minPrice = query.Price.Value - query.PriceTolerance;
                var maxPrice = query.Price.Value + query.PriceTolerance;

                foodsList = foodsList
                    .Where(x => x.Price >= minPrice && x.Price <= maxPrice)
                    .ToList();
            }

            var totalItems = foodsList.Count;

            var items = foodsList
                .OrderBy(x => x.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new FoodResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    CategoryName = x.Category.Name
                })
                .ToList();

            return new
            {
                totalItems,
                query.PageNumber,
                query.PageSize,
                items
            };
        }

        // ================= GET BY ID =================
        public async Task<FoodResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Foods
                .Include(x => x.Category)
                .Where(x => x.Id == id)
                .Select(x => new FoodResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    CategoryName = x.Category.Name
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        // ================= CREATE =================
        public async Task<FoodResponseDto> CreateAsync(FoodCreateDto dto)
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

            var categoryName = await _context.Categories
                .Where(c => c.Id == dto.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            return new FoodResponseDto
            {
                Id = food.Id,
                Name = food.Name,
                Price = food.Price,
                ImageUrl = food.ImageUrl,
                CategoryName = categoryName ?? ""
            };
        }

        // ================= PUT =================
        public async Task<bool> UpdateAsync(int id, FoodUpdateDto dto)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            food.Name = dto.Name;
            food.Price = dto.Price;
            food.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return true;
        }

        // ================= PATCH =================
        public async Task<bool> PatchAsync(int id, FoodPatchDto dto)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null) return false;

            if (dto.Name != null)
                food.Name = dto.Name;

            if (dto.Price.HasValue)
                food.Price = dto.Price.Value;

            if (dto.CategoryId.HasValue)
                food.CategoryId = dto.CategoryId.Value;


            await _context.SaveChangesAsync();
            return true;
        }

        // ================= DELETE =================
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