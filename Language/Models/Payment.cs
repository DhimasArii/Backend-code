namespace Language.Models
{
    public class Payment
    {
        public Guid id_payment_method {  get; set; }
        public string payment_name { get; set; }
        public string payment_status { get; set; }
        public string payment_description { get; set; }
    }
}
