using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class DishIngredientEntity
    {
        [JsonIgnore]
        public int dish_id { get; set; }
        [JsonIgnore]
        public int ingredient_id { get; set; }
        public string ingredientName { get; set; }
        public int count { get; set; }
        public string units { get; set; }
    }
}
