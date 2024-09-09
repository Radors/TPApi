using Azure.Security.KeyVault.Secrets;
using OpenAI.Embeddings;
using Polly.Retry;
using Polly;
using System.Numerics;
using TPApi.Food.DBModels;
using TPApi.Food.APIModels;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;

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
            // cts.Cancel();

            EmbeddingCollection newEmbeddings = await firstResponse;

            float[][] vectors = newEmbeddings.Select(e => e.Vector.ToArray()).ToArray();
            return vectors;
        }

        private async Task<EmbeddingCollection> GenerateEmbeddingsWithDelay(string[] names, int delay, CancellationToken token)
        {
            if (delay > 0) await Task.Delay(delay);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            EmbeddingClient client = new("text-embedding-3-large", _key);
            var results = await client.GenerateEmbeddingsAsync(names, null, token);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            return results;
        }

        public FoodAggregation[] GetAggregations(FoodInput[] foodInputs, float[][] newEmbeddings, 
                                                        FoodEmbedding[] storedEmbeddings, FoodProduct[] storedProducts)
        {
            FoodAggregation[] aggregations = new FoodAggregation[foodInputs.Length];

            for (int i = 0; i < foodInputs.Length; i++)
            {
                FoodAggregation aggregation = new FoodAggregation(foodInputs[i].FrontendId, foodInputs[i].Name, foodInputs[i].Weight);
                aggregations[i] = aggregation;
            }

            for (int i = 0; i < aggregations.Length; i++)
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
                    aggregations[i].Rejected = true;
                    continue;
                }
                int[] chosenProductIds = topSimilarities.Where(e => Math.Abs(e.Item2 - topSimilarities[0].Item2) < _maxSimilarityDistance).Select(e => e.Item1).ToArray();

                foreach (var id in chosenProductIds)
                {
                    var product = storedProducts.Single(e => e.Id == id);
                    int numberOfProducts = chosenProductIds.Length;
                    float weightFactor = aggregations[i].Weight > 0 ? aggregations[i].Weight/100f : product.Weight/100f;
                    if (aggregations[i].Weight < 1) aggregations[i].Weight = product.Weight;
                    aggregations[i].Jod += product.Jod / numberOfProducts * weightFactor / RecDailyIntake.Jod;
                    aggregations[i].Jarn += product.Jarn / numberOfProducts * weightFactor / RecDailyIntake.Jarn;
                    aggregations[i].Kalcium += product.Kalcium / numberOfProducts * weightFactor / RecDailyIntake.Kalcium;
                    aggregations[i].Kalium += product.Kalium / numberOfProducts * weightFactor / RecDailyIntake.Kalium;
                    aggregations[i].Magnesium += product.Magnesium / numberOfProducts * weightFactor / RecDailyIntake.Magnesium;
                    aggregations[i].Selen += product.Selen / numberOfProducts * weightFactor / RecDailyIntake.Selen;
                    aggregations[i].Zink += product.Zink / numberOfProducts * weightFactor / RecDailyIntake.Zink;
                    aggregations[i].A += product.A / numberOfProducts * weightFactor / RecDailyIntake.A;
                    aggregations[i].B1 += product.B1 / numberOfProducts * weightFactor / RecDailyIntake.B1;
                    aggregations[i].B2 += product.B2 / numberOfProducts * weightFactor / RecDailyIntake.B2;
                    aggregations[i].B3 += product.B3 / numberOfProducts * weightFactor / RecDailyIntake.B3;
                    aggregations[i].B6 += product.B6 / numberOfProducts * weightFactor / RecDailyIntake.B6;
                    aggregations[i].B9 += product.B9 / numberOfProducts * weightFactor / RecDailyIntake.B9;
                    aggregations[i].B12 += product.B12 / numberOfProducts * weightFactor / RecDailyIntake.B12;
                    aggregations[i].C += product.C / numberOfProducts * weightFactor / RecDailyIntake.C;
                    aggregations[i].D += product.D / numberOfProducts * weightFactor / RecDailyIntake.D;
                    aggregations[i].E += product.E / numberOfProducts * weightFactor / RecDailyIntake.E;
                }
            }
            return aggregations;
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