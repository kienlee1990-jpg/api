using FastFoodAPI.DTOs.Cart;

namespace FastFoodAPI.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}