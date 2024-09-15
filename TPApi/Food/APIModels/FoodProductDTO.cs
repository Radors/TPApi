using TPApi.Food.DBModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TPApi.Food.APIModels
{
    public class FoodProductDTO
    {
        // The query is received as part of any request to either of the search-endpoints
        // All returned FoodProductDTOs, relating to one request, will share the same Query
        public string Query { get; set; }
        // An id that is only utilized by the frontend
        public int FrontendId { get; set; }
        // The name of the matched item
        public string Name { get; set; }
        // Nutritional values are percentages in decimal point form
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

        public FoodProductDTO(string query, int frontendId, string name, FoodProduct foodProduct)
        {
            Query = query;
            FrontendId = frontendId;
            Name = name;
            Jod = (float)Math.Round(foodProduct.Jod / RecDailyIntake.Jod, 2);
            Jarn = (float)Math.Round(foodProduct.Jarn / RecDailyIntake.Jarn, 2);
            Kalcium = (float)Math.Round(foodProduct.Kalcium / RecDailyIntake.Kalcium, 2);
            Kalium = (float)Math.Round(foodProduct.Kalium / RecDailyIntake.Kalium, 2);
            Magnesium = (float)Math.Round(foodProduct.Magnesium / RecDailyIntake.Magnesium, 2);
            Selen = (float)Math.Round(foodProduct.Selen / RecDailyIntake.Selen, 2);
            Zink = (float)Math.Round(foodProduct.Zink / RecDailyIntake.Zink, 2);
            A = (float)Math.Round(foodProduct.A / RecDailyIntake.A, 2);
            B1 = (float)Math.Round(foodProduct.B1 / RecDailyIntake.B1, 2);
            B2 = (float)Math.Round(foodProduct.B2 / RecDailyIntake.B2, 2);
            B3 = (float)Math.Round(foodProduct.B3 / RecDailyIntake.B3, 2);
            B6 = (float)Math.Round(foodProduct.B6 / RecDailyIntake.B6, 2);
            B9 = (float)Math.Round(foodProduct.B9 / RecDailyIntake.B9, 2);
            B12 = (float)Math.Round(foodProduct.B12 / RecDailyIntake.B12, 2);
            C = (float)Math.Round(foodProduct.C / RecDailyIntake.C, 2);
            D = (float)Math.Round(foodProduct.D / RecDailyIntake.D, 2);
            E = (float)Math.Round(foodProduct.E / RecDailyIntake.E, 2);
        }
    }
}
