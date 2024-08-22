using Microsoft.EntityFrameworkCore;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Services
{
    public class EmbeddingsInMemory
    {
        private FoodEmbedding[] _embeddings = Array.Empty<FoodEmbedding>();
        private bool _isReady = false;
        private readonly IServiceScopeFactory _scopeFactory;

        public EmbeddingsInMemory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task LoadDataAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();
                _embeddings = await context.FoodEmbeddings.ToArrayAsync();
                _isReady = true;
            }
        }

        public FoodEmbedding[]? TryGetEmbeddings()
        {
            return _isReady ? _embeddings : null;
        }
    }
}
