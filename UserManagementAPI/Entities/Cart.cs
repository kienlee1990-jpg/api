namespace FastFoodAPI.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}