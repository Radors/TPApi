using Microsoft.EntityFrameworkCore;
using TPApi.Food.DBModels;

namespace TPApi.Data
{
    public class TPDbContext : DbContext
    {
        public TPDbContext(DbContextOptions<TPDbContext> options) : base(options)
        {
        }

        public DbSet<FoodEmbedding> FoodEmbeddings { get; set; }
        public DbSet<FoodProduct> FoodProducts { get; set; }
    }
}
