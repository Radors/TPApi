using OpenAI.Embeddings;
using TPApi.Data;
using TPApi.Food.DBModels;

namespace TPApi.Food
{
    public static class InputProcessor
    {
        private readonly static float maxSimilarityDistance = 0.1f;
        private readonly static int maxProductsPerAggregation = 5;

        public static async Task<float[][]> GetEmbeddingsAsync(FoodInput[] foodInputs)
        {
            List<FoodAggregation> foodAggregations = new();
            foreach (var foodInput in foodInputs)
            {
                var newAggregation = new FoodAggregation(foodInput.FrontendId, foodInput.Name, foodInput.Weight);
                foodAggregations.Add(newAggregation);
            }

            EmbeddingClient client = new("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
            EmbeddingCollection newEmbeddings = await client.GenerateEmbeddingsAsync(foodAggregations.Select(e => e.Name).ToArray());
            float[][] vectors = newEmbeddings.Select(e => e.Vector.ToArray()).ToArray();

            return vectors;
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
                foreach (var storedEmbedding in storedEmbeddings)
                {
                    float similarity = ComputeDotProduct(newEmbeddings[i], storedEmbedding.Vector);
                    similarities[i] = (storedEmbedding.Id, similarity);
                }

                (int, float)[] topSimilarities = similarities.OrderByDescending(e => e.Item2).Take(maxProductsPerAggregation).ToArray();
                int[] chosenProductIds = topSimilarities.Where(e => Math.Abs(e.Item2 - topSimilarities[0].Item2) < maxSimilarityDistance).Select(e => e.Item1).ToArray();

                foreach (var id in chosenProductIds)
                {
                    var product = storedProducts.Single(e => e.Id == id);
                    int numberOfProducts = chosenProductIds.Length;
                    int weightFactor = aggregations[i].Weight > 0 ? aggregations[i].Weight/100 : product.Weight/100;
                    aggregations[i].Jod += product.Jod / numberOfProducts * weightFactor;
                    aggregations[i].Jarn += product.Jarn / numberOfProducts * weightFactor;
                    aggregations[i].Kalcium += product.Kalcium / numberOfProducts * weightFactor;
                    aggregations[i].Kalium += product.Kalium / numberOfProducts * weightFactor;
                    aggregations[i].Magnesium += product.Magnesium / numberOfProducts * weightFactor;
                    aggregations[i].Selen += product.Selen / numberOfProducts * weightFactor;
                    aggregations[i].Zink += product.Zink / numberOfProducts * weightFactor;
                    aggregations[i].A += product.A / numberOfProducts * weightFactor;
                    aggregations[i].B1 += product.B1 / numberOfProducts * weightFactor;
                    aggregations[i].B2 += product.B2 / numberOfProducts * weightFactor;
                    aggregations[i].B3 += product.B3 / numberOfProducts * weightFactor;
                    aggregations[i].B6 += product.B6 / numberOfProducts * weightFactor;
                    aggregations[i].B9 += product.B9 / numberOfProducts * weightFactor;
                    aggregations[i].B12 += product.B12 / numberOfProducts * weightFactor;
                    aggregations[i].C += product.C / numberOfProducts * weightFactor;
                    aggregations[i].D += product.D / numberOfProducts * weightFactor;
                    aggregations[i].E += product.E / numberOfProducts * weightFactor;
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
