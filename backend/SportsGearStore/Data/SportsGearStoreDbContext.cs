using Microsoft.EntityFrameworkCore;
using SportsGearStore.Models;

namespace SportsGearStore.Data
{
    public class SportsGearStoreDbContext : DbContext
    {
        public SportsGearStoreDbContext(DbContextOptions<SportsGearStoreDbContext> options) : base(options) {}

        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
    }
}
