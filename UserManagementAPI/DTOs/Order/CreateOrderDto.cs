using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}