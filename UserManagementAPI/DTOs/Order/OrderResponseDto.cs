namespace FastFoodAPI.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}