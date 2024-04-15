using Language.DTOs.DetailCheckout;

namespace Language.DTOs.Checkout
{
    public class CheckoutDTO
    {
        public Guid checkout_id { get; set; }
        public Guid user_id {  get; set; }
        public Guid id_payment_method { get; set; }

        public List<DetailShowCheckoutDTO> checkout_detail { get; set; }


    }
}
