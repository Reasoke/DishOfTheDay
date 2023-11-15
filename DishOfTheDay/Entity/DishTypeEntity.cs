using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public class DishTypeEntity
    {
        [JsonIgnore]
        public int dish_type_id { get; set; }
        public string name { get; set; }
    }
}