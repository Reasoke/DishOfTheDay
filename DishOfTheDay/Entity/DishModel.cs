using Newtonsoft.Json;
using System.Collections.Generic;

namespace DishOfTheDay.Entity
{
    public class DishModel
    {
        public class IngredientModel
        {
            public string ingredientName { get; set; }
            public string units { get; set; }
            public int count { get; set; }
        }

        [JsonIgnore]
        public int dish_id { get; set; }
        public string name { get; set; }
        public int cooking_time { get; set; }
        public string recipe { get; set; }
        public byte[] picture { get; set; }

        public string KitchenName { get; set; }
        public string DishTypeName { get; set; }

        public List<IngredientModel> ingredients { get; set; }

        public DishModel()
        {
            ingredients = new List<IngredientModel>();
        }
    }
}
