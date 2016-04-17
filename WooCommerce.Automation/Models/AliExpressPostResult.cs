namespace WooCommerce.Automation.Models
{
    public class AliExpressPostResult
    {
        public bool Success { get; set; }
        public string PostedUrl { get; set; }
        public string SourceUrl { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
    }
}