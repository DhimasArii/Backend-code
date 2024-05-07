namespace Language.DTOs.User
{
    public class UserDTO
    {
        public string email { get; set; } = string.Empty;
        public string passwords { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool IsActivated { get; set; }
    }
}
