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
        private readonly float _maxSimilarityDistance = 0.1f;
        private readonly int _maxProductsPerAggregation = 5;
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

        public async Task<float[][]> GetEmbeddingsAsync(FoodInput[] foodInputs)
        {
            string[] names = foodInputs.Select(e => e.Name).ToArray();

            using var cts = new CancellationTokenSource();
            var token = cts.Token;
            var tasks = new[]
            {
                GenerateEmbeddingsWithDelay(names, 0, token),
                GenerateEmbeddingsWithDelay(names, 45, token),
                GenerateEmbeddingsWithDelay(names, 95, token),
                GenerateEmbeddingsWithDelay(names, 150, token)
            };
            var firstResponse = await Task.WhenAny(tasks);
            cts.Cancel();

            EmbeddingCollection newEmbeddings = await firstResponse;

            float[][] vectors = newEmbeddings.Select(e => e.Vector.ToArray()).ToArray();
            return vectors;
        }

        private async Task<EmbeddingCollection> GenerateEmbeddingsWithDelay(string[] names, int delay, CancellationToken token)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            EmbeddingClient client = new("text-embedding-3-large", _key);
            return await client.GenerateEmbeddingsAsync(names, null, token);
        }

        public FoodProductDTO[] GetFoodProductDTOs(FoodInput[] foodInputs, float[][] newEmbeddings, 
                                                        FoodEmbedding[] storedEmbeddings, FoodProduct[] storedProducts)
        {
            FoodProductDTO[] foodProductDTOs = new FoodProductDTO[foodInputs.Length];

            for (int i = 0; i < foodInputs.Length; i++)
            {
                FoodProductDTO FoodProductDTO = new FoodProductDTO(foodInputs[i].FrontendId, foodInputs[i].Name);
                foodProductDTOs[i] = FoodProductDTO;
            }

            for (int i = 0; i < foodProductDTOs.Length; i++)
            {
                (int, float)[] similarities = new (int, float)[storedProducts.Length];

                for (int j = 0; j < storedEmbeddings.Length; j++)
                {
                    float similarity = ComputeDotProduct(newEmbeddings[i], storedEmbeddings[j].Vector);
                    similarities[j] = (storedEmbeddings[j].Id, similarity);
                }

                (int, float)[] topSimilarities = similarities.OrderByDescending(e => e.Item2).Take(_maxProductsPerAggregation).ToArray();
                if (topSimilarities[0].Item2 < 0.4f)
                {
                    foodProductDTOs[i].Rejected = true;
                    continue;
                }
                int[] chosenProductIds = topSimilarities.Where(e => Math.Abs(e.Item2 - topSimilarities[0].Item2) < _maxSimilarityDistance).Select(e => e.Item1).ToArray();

                foreach (var id in chosenProductIds)
                {
                    var product = storedProducts.Single(e => e.Id == id);
                    int numberOfProducts = chosenProductIds.Length;
                    foodProductDTOs[i].Jod += product.Jod / numberOfProducts / RecDailyIntake.Jod;
                    foodProductDTOs[i].Jarn += product.Jarn / numberOfProducts / RecDailyIntake.Jarn;
                    foodProductDTOs[i].Kalcium += product.Kalcium / numberOfProducts / RecDailyIntake.Kalcium;
                    foodProductDTOs[i].Kalium += product.Kalium / numberOfProducts / RecDailyIntake.Kalium;
                    foodProductDTOs[i].Magnesium += product.Magnesium / numberOfProducts / RecDailyIntake.Magnesium;
                    foodProductDTOs[i].Selen += product.Selen / numberOfProducts / RecDailyIntake.Selen;
                    foodProductDTOs[i].Zink += product.Zink / numberOfProducts / RecDailyIntake.Zink;
                    foodProductDTOs[i].A += product.A / numberOfProducts / RecDailyIntake.A;
                    foodProductDTOs[i].B1 += product.B1 / numberOfProducts / RecDailyIntake.B1;
                    foodProductDTOs[i].B2 += product.B2 / numberOfProducts / RecDailyIntake.B2;
                    foodProductDTOs[i].B3 += product.B3 / numberOfProducts / RecDailyIntake.B3;
                    foodProductDTOs[i].B6 += product.B6 / numberOfProducts / RecDailyIntake.B6;
                    foodProductDTOs[i].B9 += product.B9 / numberOfProducts / RecDailyIntake.B9;
                    foodProductDTOs[i].B12 += product.B12 / numberOfProducts / RecDailyIntake.B12;
                    foodProductDTOs[i].C += product.C / numberOfProducts / RecDailyIntake.C;
                    foodProductDTOs[i].D += product.D / numberOfProducts / RecDailyIntake.D;
                    foodProductDTOs[i].E += product.E / numberOfProducts / RecDailyIntake.E;
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