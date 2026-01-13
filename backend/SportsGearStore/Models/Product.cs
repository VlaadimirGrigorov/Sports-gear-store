namespace SportsGearStore.Models
{
    public class Product
    {
        public int Id { get; set; } // primary key
        public int CategoryId { get; set; } // foreign key
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
