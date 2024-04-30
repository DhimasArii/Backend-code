using System;
namespace Language.Models
{
    public class Checkout
    {
        public Guid checkout_id { get; set; }
        public Guid user_id { get; set; }
        public Guid id_payment_method { get; set; }
        public DateTime? create_date { get; set; }

        public List<Detail_Checkout> checkout_detail { get; set; }



    }
}
