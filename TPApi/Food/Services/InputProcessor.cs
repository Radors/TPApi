using Azure.Security.KeyVault.Secrets;
using OpenAI.Embeddings;
using Polly.Retry;
using Polly;
using System.Numerics;
using TPApi.Food.DBModels;
using TPApi.Food.APIModels;

namespace TPApi.Food
{
    public class InputProcessor
    {
        private readonly float _maxSimilarityDistance = 0.25f;
        private readonly int _maxProductsPerSearch = 14;
        private readonly string _key;

        public InputProcessor(SecretClient secretClient)
        {
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                    .AddRetry(new RetryStrategyOptions()
                    {
                        MaxRetryAttempts = 3,
                    })
                    .AddTimeout(TimeSpan.FromSeconds(3))
                    .Build();

            string key = string.Empty;

            pipeline.Execute(token =>
            {
                var secret = secretClient.GetSecret("openai-api-key", null, token);
                if (secret is not null) 
                {
                    key = secret.Value.Value;
                }
            }, CancellationToken.None);

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("Failed to retrieve key");
            }
            _key = key;
        }

        public async Task<float[]> GetEmbeddingAsync(string query)
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;
            var tasks = new[]
            {
                GenerateEmbeddingWithDelay(query, 0, token),
                GenerateEmbeddingWithDelay(query, 45, token),
                GenerateEmbeddingWithDelay(query, 95, token),
            };
            var firstResponse = await Task.WhenAny(tasks);
            cts.Cancel();

            Embedding newEmbedding = await firstResponse;

            float[] vector = newEmbedding.Vector.ToArray();
            return vector;
        }

        private async Task<Embedding> GenerateEmbeddingWithDelay(string query, int delay, CancellationToken token)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            EmbeddingClient client = new("text-embedding-3-large", _key);
            return await client.GenerateEmbeddingAsync(query, null, token);
        }

        public FoodProductDTO[] GetFoodProductDTOs(string query, float[] newEmbedding, FoodEmbedding[] storedEmbeddings, 
                                                   FoodProduct[] storedProducts, int frontendId)
        {
            (int id, float similarity)[] similarities = new (int, float)[storedProducts.Length];

            for (int i = 0; i < storedEmbeddings.Length; i++)
            {
                float similarity = ComputeDotProduct(newEmbedding, storedEmbeddings[i].Vector);
                similarities[i] = (storedEmbeddings[i].Id, similarity);
            }

            (int id, float similarity)[] topSimilarities = similarities.OrderByDescending(e => e.Item2).Take(_maxProductsPerSearch).ToArray();

            topSimilarities = topSimilarities.Where(e => e.similarity > 0.4f).ToArray();
            if (topSimilarities.Length == 0)
            {
                return Array.Empty<FoodProductDTO>();
            }
            int[] chosenProductIds = topSimilarities.Where(e => Math.Abs(e.Item2 - topSimilarities[0].Item2) < _maxSimilarityDistance).Select(e => e.Item1).ToArray();

            FoodProductDTO[] foodProductDTOs = new FoodProductDTO[chosenProductIds.Length];

            for (int i = 0; i < chosenProductIds.Length; i++)
            {
                var product = storedProducts.Single(e => e.Id == chosenProductIds[i]);
                if (product is FoodProduct foodProduct)
                {
                    FoodProductDTO foodProductDTO = new FoodProductDTO(query, frontendId, foodProduct.Name, foodProduct);
                    foodProductDTOs[i] = foodProductDTO;
                }
            }
            return foodProductDTOs;
        }

        private float ComputeDotProduct(float[] vectorA, float[] vectorB)
        {
            float sum = 0;
            int length = vectorA.Length;
            int simdLength = Vector<float>.Count;
            int i = 0;
            for (; i <= length - simdLength; i += simdLength)
            {
                var v1 = new Vector<float>(vectorA, i);
                var v2 = new Vector<float>(vectorB, i);
                sum += Vector.Dot(v1, v2);
            }
            for (; i < length; i++)
            {
                sum += vectorA[i] * vectorB[i];
            }
            return sum;
        }
    }
}