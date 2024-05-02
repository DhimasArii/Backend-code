namespace Language.Models
{
    public class Detail_Invoice
    {
        public Guid detail_invoice_id { get; set; }
        public Guid invoice_id { get; set; }
        public Guid schedule_id { get; set; }


        //info tambahan dari schedule_id
        public string category_name { get; set; }
        public string course_name { get; set; }
        public int course_price { get; set; }
        public DateTime course_date { get; set; }
    }
}
