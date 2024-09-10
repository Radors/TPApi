using System.ComponentModel.DataAnnotations;

namespace TPApi.Food.APIModels
{
    public class FoodInput
    {
        public required int FrontendId { get; set; }
        public required string Name { get; set; }
    }
}
