namespace Language.Models
{
    public class Detail_Invoice
    {
        public Guid detail_invoice_id { get; set; }
        public Guid invoice_id { get; set; }
        public Guid schedule_id { get; set; }
    }
}
