using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using Polly;
using System.Diagnostics;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Services
{
    public class ProductsInMemory
    {
        private FoodProduct[] _products = Array.Empty<FoodProduct>();
        private bool _isReady = false;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductsInMemory> _logger;

        public ProductsInMemory(IServiceScopeFactory scopeFactory, ILogger<ProductsInMemory> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task LoadDataAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TPDbContext>();

                ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                    .AddRetry(new RetryStrategyOptions()
                    {
                        MaxRetryAttempts = 3,
                    })
                    .AddTimeout(TimeSpan.FromSeconds(20))
                    .Build();

                await pipeline.ExecuteAsync(async token =>
                {
                    _products = await context.FoodProducts.ToArrayAsync(token);
                    if (_products != Array.Empty<FoodProduct>()) _isReady = true;
                }, CancellationToken.None);
            }
            stopwatch.Stop();
            _logger.LogInformation("ProductsInMemory.LoadDataAsync took {duration} ms", stopwatch.ElapsedMilliseconds);
        }

        public FoodProduct[]? TryGetProducts()
        {
            return _isReady ? _products : null;
        }
    }
}