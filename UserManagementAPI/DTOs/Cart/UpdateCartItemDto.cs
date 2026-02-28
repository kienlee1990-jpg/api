namespace FastFoodAPI.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}