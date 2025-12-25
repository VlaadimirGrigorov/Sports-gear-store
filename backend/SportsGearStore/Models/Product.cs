namespace SportsGearStore.Models
{
    public class Product
    {
        public int Id { get; set; } // primary key
        public int CategoryId { get; set; } // foreign key
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
