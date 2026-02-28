using FastFoodAPI.DTOs.Order;
using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ⭐ QUAN TRỌNG: phải khởi tạo
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}