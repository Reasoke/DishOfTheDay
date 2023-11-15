using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class IngredientEntity
    {
        [JsonIgnore]
        public int ingredient_id { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public string units { get; set; }
        public decimal expiration { get; set; }
        public string manufacturer { get; set; }
    }

}
