namespace SportsGearStore.Models
{
    public class Cart
    {
        public int Id { get; set; } // primary key
        public int UserId { get; set; } // foreign key
        public List<CartItem> Items { get; set; } = new();
    }
}
