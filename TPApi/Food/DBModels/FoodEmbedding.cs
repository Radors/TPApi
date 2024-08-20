namespace TPApi.Food.DBModels
{
    public class FoodEmbedding
    {
        public int Id { get; set; }
        // OldId originates from the second column of the raw data: "Livsmedelsnummer".
        public required int OldId { get; set; }
        public required float[] Vector { get; set; }
    }
}
