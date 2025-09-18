namespace OfficeService.DAL.Models
{
    public class AppSetting
    {
        public string DocumentServerSecret { get; set; }
        public string DocumentServer { get; set; }
        public string InternalKey { get; set; }
        public string CallbackUrl { get; set; }
        public int JobInterval { get; set; }
    }
}
