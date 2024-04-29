namespace Language.Models
{
    public class Detail_Checkout
    {
        public Guid detail_checkout_id { get; set; }
        public Guid checkout_id { get; set; }
        public Guid schedule_id { get; set; }
        public bool checklist { get; set; }

        //info tambahan dari schedule_id
        public string category_name { get; set; }
        public string course_name { get; set; }
        public DateTime course_date { get; set; }
        public string course_description { get; set; }
        public string course_image { get; set;}
        public int price { get; set;}
    }
}
