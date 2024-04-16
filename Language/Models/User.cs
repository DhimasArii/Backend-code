namespace Language.Models
{
    public class User
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string passwords { get; set; }
        public string address { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty ;
       

    }
}
