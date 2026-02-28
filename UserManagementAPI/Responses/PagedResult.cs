namespace FastFoodAPI.Responses
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PagedResult() { }

        public PagedResult(List<T> items, int total, int page, int pageSize)
        {
            Items = items;
            TotalCount = total;
            Page = page;
            PageSize = pageSize;
        }
    }
}