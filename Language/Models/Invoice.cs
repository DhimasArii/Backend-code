namespace Language.Models
{
    public class Invoice
    {
        public Guid invoice_id {  get; set; }
        public Guid checkout_id { get; set; }
        public int no_invoice {  get; set; }
        public DateTime tanggal {  get; set; }
        public int total_price { get; set; }

    }
}
