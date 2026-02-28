using FastFoodAPI.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    // ⭐ cho phép null
    public int? FoodId { get; set; }
    public int? ComboId { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Order? Order { get; set; }
    public Food? Food { get; set; }
    public Combo? Combo { get; set; }
}