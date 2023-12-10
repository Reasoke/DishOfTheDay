using Newtonsoft.Json;

namespace DishOfTheDay.Entity
{
    public enum ClientRole
    {
        User = 0,
        Admin = 1,
    }

    public class ClientEntity
    {
        [JsonIgnore]
        public int client_id { get; set; }
        [JsonIgnore]
        public ClientRole role { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string description { get; set; }
    }
}
