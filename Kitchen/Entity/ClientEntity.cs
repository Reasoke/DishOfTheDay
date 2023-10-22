using System;

namespace Kitchen
{
    public class ClientEntity
    {
        public int client_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string description { get; set; }
        public DateTime DOB { get; set; }
    }

}
