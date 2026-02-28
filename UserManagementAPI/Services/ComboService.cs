using FastFoodAPI.Data;
using FastFoodAPI.DTOs.Combo;
using FastFoodAPI.Entities;
using FastFoodAPI.Helpers;
using FastFoodAPI.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FastFoodAPI.Services
{
    public class ComboService : IComboService
    {
        private readonly ApplicationDbContext _context;

        public ComboService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= CREATE =================
        public async Task<bool> CreateAsync(ComboCreateDto dto)
        {
            if (dto.Foods == null || !dto.Foods.Any())
                throw new Exception("Combo phải có ít nhất 1 món");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var combo = new Combo
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description
                };

                _context.Combos.Add(combo);
                await _context.SaveChangesAsync();

                var comboFoods = dto.Foods.Select(x => new ComboFood
                {
                    ComboId = combo.Id,
                    FoodId = x.FoodId,
                    Quantity = x.Quantity
                });

                _context.ComboFoods.AddRange(comboFoods);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================= GET PAGED =================
        public async Task<object> GetPagedAsync(ComboQueryParams query)
        {
            var combosQuery = _context.Combos
                .AsNoTracking()
                .Include(c => c.ComboFoods)
                .ThenInclude(cf => cf.Food)
                .AsQueryable();

            var comboList = await combosQuery.ToListAsync();

            // 🔍 search không dấu
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = StringHelper.RemoveDiacritics(query.Search).ToLower();

                comboList = comboList
                    .Where(c =>
                        StringHelper.RemoveDiacritics(c.Name).ToLower().Contains(keyword))
                    .ToList();
            }

            var totalItems = comboList.Count;

            var items = comboList
                .OrderByDescending(x => x.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new ComboResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    Foods = c.ComboFoods.Select(x => new ComboFoodItemDto
                    {
                        FoodId = x.FoodId,
                        FoodName = x.Food.Name,
                        Quantity = x.Quantity
                    }).ToList()
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
        public async Task<ComboResponseDto?> GetByIdAsync(int id)
        {
            var combo = await _context.Combos
                .AsNoTracking()
                .Include(c => c.ComboFoods)
                .ThenInclude(cf => cf.Food)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (combo == null) return null;

            return new ComboResponseDto
            {
                Id = combo.Id,
                Name = combo.Name,
                Price = combo.Price,
                Foods = combo.ComboFoods.Select(x => new ComboFoodItemDto
                {
                    FoodId = x.FoodId,
                    FoodName = x.Food.Name,
                    Quantity = x.Quantity
                }).ToList()
            };
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(int id, ComboCreateDto dto)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboFoods)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (combo == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                combo.Name = dto.Name;
                combo.Price = dto.Price;
                combo.Description = dto.Description;

                // ❌ xóa món cũ
                _context.ComboFoods.RemoveRange(combo.ComboFoods);
                await _context.SaveChangesAsync();

                // ✅ thêm món mới
                var newFoods = dto.Foods.Select(x => new ComboFood
                {
                    ComboId = combo.Id,
                    FoodId = x.FoodId,
                    Quantity = x.Quantity
                });

                await _context.ComboFoods.AddRangeAsync(newFoods);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null) return false;

            _context.Combos.Remove(combo);
            await _context.SaveChangesAsync();
            return true;
        }

        // ================= CALCULATE STOCK =================
        public async Task<int> CalculateComboStockAsync(int comboId)
        {
            var comboFoods = await _context.ComboFoods
                .Include(x => x.Food)
                .Where(x => x.ComboId == comboId)
                .ToListAsync();

            if (!comboFoods.Any())
                return 0;

            int minStock = int.MaxValue;

            foreach (var cf in comboFoods)
            {
                var possible = cf.Food.StockQuantity / cf.Quantity;
                minStock = Math.Min(minStock, possible);
            }

            return minStock;
        }

        // ================= UPDATE COMBOS BY FOOD (SQL) =================
        public async Task UpdateCombosByFoodSqlAsync(int foodId)
        {
            var sql = @"
        UPDATE c
        SET StockQuantity = sub.MinStock
        FROM Combos c
        JOIN (
            SELECT
                cf.ComboId,
                MIN(f.StockQuantity / cf.Quantity) AS MinStock
            FROM ComboFoods cf
            JOIN Foods f ON f.Id = cf.FoodId
            WHERE cf.ComboId IN (
                SELECT ComboId
                FROM ComboFoods
                WHERE FoodId = @foodId
            )
            GROUP BY cf.ComboId
        ) sub ON sub.ComboId = c.Id
    ";

            await _context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@foodId", foodId));
        }
    }
}