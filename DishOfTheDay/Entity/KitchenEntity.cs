using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class KitchenEntity
    {
        [JsonIgnore]
        public int kitchen_id { get; set; }
        public string name { get; set; }
        public byte[] country_flag { get; set; }
    }
}