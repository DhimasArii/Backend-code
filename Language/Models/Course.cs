﻿namespace Language.Models
{
    public class Course
    {
        public Guid course_id { get; set; }
        public Guid category_id { get; set; }
        public string course_name { get; set; }
        public string course_description { get; set;}
        public int price { get; set; }
    }
}