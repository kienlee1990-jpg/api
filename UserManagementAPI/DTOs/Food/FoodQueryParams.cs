using Microsoft.AspNetCore.Mvc.ModelBinding;

public class FoodQueryParams
{
    public string? Search { get; set; }
    public bool? IsAvailable { get; set; }
    public int? CategoryId { get; set; }

    // 🔥 thêm
    public decimal? Price { get; set; }      // giá user nhập
    public decimal PriceTolerance { get; set; } = 5000; // sai số

    [BindNever]
    public int PageNumber { get; set; } = 1;
    [BindNever]
    public int PageSize { get; set; } = 10;
}