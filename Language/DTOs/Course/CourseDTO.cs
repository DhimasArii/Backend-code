namespace Language.DTOs.Course
{
    public class CourseDTO
    {
        public Guid category_id { get; set; }
        public string course_name { get; set; }
        public string course_description { get; set;}
        public string course_image { get; set;}
        public int price { get; set; }
    }
}
