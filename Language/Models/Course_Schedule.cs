namespace Language.Models
{
    public class Course_Schedule
    {
        public Guid schedule_id { get; set; }
        public Guid course_id { get; set; }
        public DateTime course_date { get; set; }
    }
}
