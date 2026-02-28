public class ComboResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public List<ComboFoodItemDto> Foods { get; set; } = new();
}