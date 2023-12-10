namespace DishOfTheDay.Entity
{
    public class ClientDishEntity
    {
        public int client_id { get; set; }
        public int dish_id { get; set; }
        public int rating { get; set; }
        public int usage_count { get; set; }
        public string review { get; set; }
    }
}
