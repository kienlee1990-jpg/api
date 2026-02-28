namespace FastFoodAPI.Interfaces;

public interface ICheckoutService
{
    Task<int> CheckoutAsync(string userId);
}