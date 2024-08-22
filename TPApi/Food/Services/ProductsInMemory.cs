using TPApi.Data;
using TPApi.Food.DBModels;
using Microsoft.EntityFrameworkCore;

namespace TPApi.Food.Services
{
    public class ProductsInMemory
    {
        private FoodProduct[] _products = Array.Empty<FoodProduct>();
        private bool _isReady = false;
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductsInMemory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task LoadDataAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();
                _products = await context.FoodProducts.ToArrayAsync();
                _isReady = true;
            }
        }

        public FoodProduct[]? TryGetProducts()
        {
            return _isReady ? _products : null;
        }
    }
}
