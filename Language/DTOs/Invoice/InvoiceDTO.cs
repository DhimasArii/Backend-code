namespace Language.DTOs.Invoice
{
    public class InvoiceDTO
    {
        public Guid checkout_id { get; set; }
        public int no_invoice {  get; set; }
        public DateTime tanggal {  get; set; }
        public int total_price { get; set; }
    }
}
