using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation ⭐ BẮT BUỘC
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}