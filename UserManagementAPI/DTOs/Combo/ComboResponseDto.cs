namespace FastFoodAPI.DTOs.Combo
{
    public class ComboResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public List<string> Foods { get; set; }
    }
}