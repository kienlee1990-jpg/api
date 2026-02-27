namespace FastFoodAPI.DTOs
{
    public class CartItemDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}