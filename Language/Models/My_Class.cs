
namespace Language.Models
{
    public class My_Class
    {
        internal object myclass;

        public Guid class_id { get; set; }
        public Guid user_id { get; set; }
        public Guid detail_invoice_id { get; set; }
        public List<Course> my_class { get; internal set; }
        public Guid schedule_id { get; internal set; }
        public int total_price { get; internal set; }
    }
}
