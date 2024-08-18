using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Services
{
    public class EmbeddingsInMemory
    {
        private readonly (int, float[])[] _vectors;

        public EmbeddingsInMemory(TPDbContext context)
        {
            // Get the all items in the DbSet 'FoodEmbeddings' and save them into _vectors to have them ready in-memory as a singleton

            // Implement logic to try again if the DB doesn't respond within a few seconds
        }
    }
}
