namespace FastFoodAPI.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        // ===== FK Cart =====
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        // ===== FK Food =====
        public int FoodId { get; set; }
        public Food Food { get; set; }

        public int Quantity { get; set; }
    }
}