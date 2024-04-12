using System;
namespace Language.Models
{
    public class User
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }

    }
}
