namespace Language.DTOs.DetailCheckout
{
    public class DetailCheckoutDTO
    {

        public Guid checkout_id { get; set; }
        public Guid schedule_id { get; set; }
        public bool checklist { get; set; }
    }
}
