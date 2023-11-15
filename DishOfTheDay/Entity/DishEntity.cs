using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class DishEntity
    {
        [JsonIgnore]
        public int dish_id { get; set; }
        public string name { get; set; }
        public int kitchen { get; set; }
        public int dish_type { get; set; }
        public int cooking_time { get; set; }
        public string recipe { get; set; }
        public byte[] picture { get; set; }
    }
}
