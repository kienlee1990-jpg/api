namespace FastFoodAPI.DTOs.Food
{
    public class FoodPatchDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsAvailable { get; set; }
    }
}