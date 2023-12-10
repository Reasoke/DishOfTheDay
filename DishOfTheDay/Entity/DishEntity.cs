using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class DishEntity
    {
        [JsonIgnore]
        public int dish_id { get; set; }
        public string name { get; set; }
        [JsonIgnore]
        public int kitchen { get; set; }
        public string KitchenName { get; set; }
        [JsonIgnore]
        public int dish_type { get; set; }
        public string DishTypeName { get; set; }
        public int cooking_time { get; set; }
        [JsonIgnore]
        public int ingredient_count { get; set; }
        [JsonIgnore]
        public decimal rating { get; set; }
        [JsonIgnore]
        public int usage_count { get; set; }
        public string recipe { get; set; }
        public byte[] picture { get; set; }
        [JsonIgnore]
        public int owner { get; set; }
        
    }
}
