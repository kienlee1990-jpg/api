using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities
{
    public class Combo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        // ✅ tồn kho combo
        public int StockQuantity { get; set; }
        // ⭐ BẮT BUỘC phải có cái này
        public bool IsAvailable { get; set; } = true;

        // ⭐ anti oversell (khuyên dùng)
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<ComboFood>? ComboFoods { get; set; }
    }
}