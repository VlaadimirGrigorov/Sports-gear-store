namespace SportsGearStore.Models
{
    public class CartItem
    {
        public int Id { get; set; } // primary key
        public int CartId { get; set; } // foreign key
        public int ProductId { get; set; } // foreign key
        public int Quantity { get; set; }
    }
}
