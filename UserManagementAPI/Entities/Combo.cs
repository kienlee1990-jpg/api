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

        public ICollection<ComboFood>? ComboFoods { get; set; }
    }
}