namespace Language.Models
{
    public class Course_Schedule
    {
        public Guid schedule_id { get; set; }
        public Guid course_id { get; set; }
        public DateTime course_date { get; set; }

        //tambahan dari table course & category 
        public string category_name { get; set; }
        public string course_name { get; set; }
        public string course_image { get; set; }
        public int price { get; set; }
    }
}
