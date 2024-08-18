namespace TPApi.Food.DBModels
{
    public class FoodEmbedding
    {
        // Id originates from the second column of the raw data: "Livsmedelsnummer".
        public required int Id { get; set; }
        public required float[] Vector { get; set; }
    }
}
