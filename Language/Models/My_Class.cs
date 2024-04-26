
namespace Language.Models
{
    public class My_Class
    {
        internal object myclass;

        public Guid class_id { get; set; }
        public Guid user_id { get; set; }
        public Guid course_id { get; set; }
        public List<Course> my_class { get; internal set; }
    }
}
