﻿namespace Language.Models
{
    public class Course
    {
        public Guid course_id { get; set; }
        public Guid category_id { get; set; }
        public string category_name { get; set; }
        public string course_name { get; set; }
        public string course_description { get; set;}

        public string course_image {  get; set;}
        public int price { get; set; }
        public int course_price { get; internal set; }
        public DateTime course_date { get; internal set; }
    }
}
