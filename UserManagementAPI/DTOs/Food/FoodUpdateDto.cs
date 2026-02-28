namespace FastFoodAPI.DTOs.Food
{
    public class FoodUpdateDto
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; }
    }
}