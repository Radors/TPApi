using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using Polly;
using System.Diagnostics;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food.Services
{
    public class EmbeddingsInMemory
    {
        private FoodEmbedding[] _embeddings = Array.Empty<FoodEmbedding>();
        private bool _isReady = false;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmbeddingsInMemory> _logger;

        public EmbeddingsInMemory(IServiceScopeFactory scopeFactory, ILogger<EmbeddingsInMemory> logger)
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
                        MaxRetryAttempts = 2,
                    })
                    .AddTimeout(TimeSpan.FromSeconds(8))
                    .Build();

                await pipeline.ExecuteAsync(async token =>
                {
                    _embeddings = await context.FoodEmbeddings.ToArrayAsync(token);
                    _isReady = true;
                }, CancellationToken.None);
            }
            stopwatch.Stop();
            _logger.LogInformation("EmbeddingsInMemory.LoadDataAsync took {Duration} ms", stopwatch.ElapsedMilliseconds);
        }

        public FoodEmbedding[]? TryGetEmbeddings()
        {
            return _isReady ? _embeddings : null;
        }
    }
}
