namespace Language.Models
{
    public class Invoice
    {
        public Guid invoice_id {  get; set; }
        public Guid user_id { get; set; }
        public string invoice_number {  get; set; }
        public DateTime invoice_date {  get; set; }
        public int total_price { get; set; }

        public List<Detail_Invoice> detail_Invoices { get; set; }

    }
}
