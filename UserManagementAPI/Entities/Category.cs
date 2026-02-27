using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        // navigation
        public ICollection<Food>? Foods { get; set; }
    }
}