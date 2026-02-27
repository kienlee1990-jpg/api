namespace FastFoodAPI.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // ===== FK Order =====
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // ===== FK Food =====
        public int FoodId { get; set; }
        public Food Food { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}