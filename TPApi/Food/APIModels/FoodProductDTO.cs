namespace TPApi.Food.APIModels
{
    // Each instance of FoodProductDTO represents the response to one instance of FoodInput
    // When a request is received, one instance of FoodProductDTO is created for each search term and at first, only .FrontendId and .Name are populated.
    public class FoodProductDTO
    {
        // An Id that is only utilized by the frontend.
        public int FrontendId { get; set; }
        // Original input Name
        public string Name { get; set; }
        // Rejected is set to true if highest similarity is below 0.4
        public bool Rejected { get; set; } = false;
        // Nutritional values are percentages in decimal form
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

        public FoodProductDTO(int frontendId, string name)
        {
            FrontendId = frontendId;
            Name = name;
        }
    }
}
