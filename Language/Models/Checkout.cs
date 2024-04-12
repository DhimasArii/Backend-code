namespace Language.Models
{
    public class Checkout
    {
        public Guid checkout_id {  get; set; }
        public Guid user_id { get; set; }
        public Guid id_payment_method { get; set; }

        public ICollection<Detail_Checkout> detail_checkout { get; set; }
    }
}
