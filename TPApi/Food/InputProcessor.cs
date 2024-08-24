using OpenAI.Embeddings;
using TPApi.Food.DBModels;

namespace TPApi.Food
{
    public static class InputProcessor
    {
        private readonly static float maxSimilarityDistance = 0.1f;
        private readonly static int maxProductsPerAggregation = 5;

        public static async Task<float[][]> GetEmbeddingsAsync(FoodInput[] foodInputs)
        {
            string[] names = foodInputs.Select(e => e.Name).ToArray();

            var tasks = new[]
            {
                GenerateEmbeddingsWithDelay(names, 0),
                GenerateEmbeddingsWithDelay(names, 45),
                GenerateEmbeddingsWithDelay(names, 95),
                GenerateEmbeddingsWithDelay(names, 150)
            };
            var firstResponse = await Task.WhenAny(tasks);
            EmbeddingCollection newEmbeddings = await firstResponse;

            float[][] vectors = newEmbeddings.Select(e => e.Vector.ToArray()).ToArray();
            return vectors;
        }

        public static async Task<EmbeddingCollection> GenerateEmbeddingsWithDelay(string[] names, int delay)
        {
            if (delay > 0) await Task.Delay(delay);
            EmbeddingClient client = new("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
            return await client.GenerateEmbeddingsAsync(names);
        }

        public static FoodAggregation[] GetAggregations(FoodInput[] foodInputs, float[][] newEmbeddings, 
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

                (int, float)[] topSimilarities = similarities.OrderByDescending(e => e.Item2).Take(maxProductsPerAggregation).ToArray();
                int[] chosenProductIds = topSimilarities.Where(e => Math.Abs(e.Item2 - topSimilarities[0].Item2) < maxSimilarityDistance).Select(e => e.Item1).ToArray();

                foreach (var id in chosenProductIds)
                {
                    var product = storedProducts.Single(e => e.Id == id);
                    int numberOfProducts = chosenProductIds.Length;
                    float weightFactor = aggregations[i].Weight > 0 ? aggregations[i].Weight/100f : product.Weight/100f;
                    if (aggregations[i].Weight == 0) aggregations[i].Weight = product.Weight;
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



        public static float ComputeDotProduct(float[] vectorA, float[] vectorB)
        {
            float dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }
            return dotProduct;
        }
    }
}
