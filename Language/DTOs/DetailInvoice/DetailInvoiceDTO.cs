namespace Language.DTOs.DetailInvoice
{
    public class DetailInvoiceDTO
    {
        public Guid schedule_id { get; set; }
        public Guid user_id { get; set; }
        public Guid id_payment_method { get; set; }
    }
}
