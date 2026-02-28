namespace FastFoodAPI.DTOs.Cart
{
    public class AddToCartDto
    {
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}