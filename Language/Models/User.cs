namespace Language.Models
{
    public class User
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string passwords { get; set; }
        public string role { get; set; }
        public bool IsActivated { get; set; }

    }
}
