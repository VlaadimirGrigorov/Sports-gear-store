using Microsoft.EntityFrameworkCore;
using SportsGearStore.Models;

namespace SportsGearStore.Data
{
    public class SportsGearStoreDbContext : DbContext
    {
        public SportsGearStoreDbContext(DbContextOptions<SportsGearStoreDbContext> options) : base(options) {}

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ProductTag> ProductTags => Set<ProductTag>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserProfileTag> UserProfileTags => Set<UserProfileTag>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<UserProfileTag>()
                .HasKey(upt => new { upt.UserProfileId, upt.TagId });
        }
    }
}
