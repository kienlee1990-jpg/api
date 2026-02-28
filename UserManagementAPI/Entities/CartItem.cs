using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        // ===== FK Cart =====
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        // ===== FK Food =====
        public int? FoodId { get; set; }
        public Food? Food { get; set; }

        // ===== FK Combo =====
        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }

        public int Quantity { get; set; }
    }
}