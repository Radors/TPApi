using System.ComponentModel.DataAnnotations;

namespace TPApi.Food
{
    public class FoodInput
    {
        public required int FrontendId { get; set; }
        public required string Name { get; set; }
        public required int Weight { get; set; }
    }
}
