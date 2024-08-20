namespace TPApi.Food.DBModels
{
    public class FoodProduct
    {
        public int Id { get; set; }
        // OldId originates from the second column of the raw data: "Livsmedelsnummer".
        public required int OldId { get; set; }
        // Name originates from the first column of the raw data: "Livsmedelsnamn"
        public required string Name { get; set; }
        // Nutritional values are in whichever unit they have sufficiently few decimals to be convenient in, the same as in the raw data.
        public float Jod { get; set; }
        public float Jarn { get; set; }
        public float Kalcium { get; set; }
        public float Kalium { get; set; }
        public float Magnesium { get; set; }
        public float Selen { get; set; }
        public float Zink { get; set; }
        public float A { get; set; }
        public float B1 { get; set; }
        public float B2 { get; set; }
        public float B3 { get; set; }
        public float B6 { get; set; }
        public float B9 { get; set; }
        public float B12 { get; set; }
        public float C { get; set; }
        public float D { get; set; }
        public float E { get; set; }
    }
}
