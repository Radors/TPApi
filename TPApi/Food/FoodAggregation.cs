namespace TPApi.Food
{
    // Each instance of FoodAggregation represents the response to one instance of FoodInput
    // The nutritional values of this element may be an aggregate of several FoodItems or just one, depending on search algorithm results.
    // When a request is received, one instance of FoodAggregation is created for each search term and at first, only Id, Name and Weight are populated.
    public class FoodAggregation
    {
        // An arbitrary unique Id that is only utilized by the frontend.
        public int FrontendId { get; set; }
        // Original input Name just carried along (not the name of any matched item!)
        public string Name { get; set; }
        // Weight may be zero after instatiation if no custom weight was received. Zero indicates to include this FoodAggregation in the request for embeddings.
        public int Weight { get; set; }
        // All aggregations of nutritional values will be ready to use: They will already be multiplied with the weight and rounded to two decimals.
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

        public FoodAggregation(int frontendId, string name, int weight)
        {
            FrontendId = frontendId;
            Name = name;
            Weight = weight;
        }
    }
}
