namespace Language.Models
{
    public class UserRole
    {
        public int id_user_role {  get; set; }
        public Guid user_id { get; set; }
        public string role {  get; set; }
    }
}
