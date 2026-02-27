using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
    }
}