namespace Language.Models
{
    public class Detail_Checkout
    {
        public Guid detail_checkout_id { get; set; }
        public Guid checkout_id { get; set; }
        public Guid course_id { get; set; }
        public bool checklist { get; set; }
    }
}
