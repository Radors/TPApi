using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Services
{
    public class ProductsInMemory
    {
        private readonly (int, FoodProduct)[] _products;

        public ProductsInMemory(TPDbContext context)
        {
            // Get the all items in the DbSet 'FoodProducts' and save them into _products to have them ready in-memory as a singleton

            // Implement logic to try again if the DB doesn't respond within a few seconds
        }
    }
}
