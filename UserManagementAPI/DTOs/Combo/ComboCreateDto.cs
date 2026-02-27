using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.DTOs.Combo
{
    public class ComboCreateDto
    {
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public List<ComboFoodItemDto> Foods { get; set; }
    }

    public class ComboFoodItemDto
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }
    }
}