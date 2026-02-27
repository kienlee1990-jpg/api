using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.DTOs.Food
{
    public class FoodCreateDto
    {
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
    }
}