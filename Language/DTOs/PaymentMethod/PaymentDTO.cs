namespace Language.DTOs.PaymentMethod
{
    public class PaymentDTO
    {
        public string payment_name {  get; set; }
        public bool payment_status { get; set;}
        public string payment_description { get; set;}
        public string payment_icon { get; set;}
    }
}
