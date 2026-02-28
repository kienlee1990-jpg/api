using System.ComponentModel.DataAnnotations;

public class FoodCreateDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = default!;

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Url]
    public string? ImageUrl { get; set; }
}