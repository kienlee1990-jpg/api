using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ComboQueryParams
{
    public string? Search { get; set; }
    [BindNever]
    public int PageNumber { get; set; } = 1;
    [BindNever]
    public int PageSize { get; set; } = 10;
}