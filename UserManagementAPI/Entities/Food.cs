using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodAPI.Entities
{
    public class Food
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        // ===== FK =====
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        // ===== combo relation =====
        public ICollection<ComboFood>? ComboFoods { get; set; }
        public bool IsAvailable { get; set; } = true;

        // ✅ tồn kho
        public int StockQuantity { get; set; }
    }
}