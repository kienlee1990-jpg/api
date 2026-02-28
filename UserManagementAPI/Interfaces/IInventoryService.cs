public interface IInventoryService
{
    Task StockInAsync(int foodId, int quantity);
}