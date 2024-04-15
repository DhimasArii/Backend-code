namespace Language.DTOs.DetailCheckout
{
    public class DetailShowCheckoutDTO
    {
        public Guid detail_checkout_id {  get; set; }
        public Guid course_id { get; set; }
        public bool checklist { get; set; }
        public string category_course { get; set; }
        public string course_name { get; set; }
        public DateTime course_date { get; set; }
    }
}
