namespace Language.DTOs.Invoice
{
    public class InvoiceDTO
    {
        public Guid user_id { get; set; }
        public Guid checkout_id { get; set; }
        public Guid id_payment_method { get; set; }
        //public int invoice_number {  get; set; }
        //public DateTime invoice_date {  get; set; }
        //public int total_price { get; set; }
    }
}
