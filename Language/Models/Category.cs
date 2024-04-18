namespace Language.Models
{
    public class Category
    {
        public Guid category_id { get; set; }
        public string category_name { get; set;}
        public string category_description { get; set;}

        public string category_image {  get; set;}

        public List<Course> courses { get; set;}
    }
}
