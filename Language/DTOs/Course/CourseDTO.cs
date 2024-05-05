
namespace Language.DTOs.Course
{
    public class CourseDTO
    {
        internal Guid course_id;
        internal Guid category_id;

        public string course_name { get; set; }
        public string course_description { get; set;}
        public string course_image { get; set;}
        public int price { get; set; }
    }
}
