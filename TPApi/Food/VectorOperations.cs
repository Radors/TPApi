namespace TPApi.Food
{
    public static class VectorOperations
    {
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
