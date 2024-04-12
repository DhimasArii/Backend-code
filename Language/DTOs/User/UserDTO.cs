namespace Language.DTOs.User
{
    public class UserDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public ICollection<Models.Checkout> checkout { get; }
        public ICollection<Models.My_Class> my_class { get; }
    }
}
