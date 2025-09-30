namespace OfficeService.DAL.Models
{
    public class ConfigAPI
    {
        public AuthAPI AuthAPI { get; set; }
        public DocumentServerAPI DocumentServerAPI { get; set; }
    }

    public abstract class BaseAPI
    {
        public string Endpoint { get; set; }
    }
    public class AuthAPI : BaseAPI
    {
        public string GetApplication { get; set; }
    }
    public class DocumentServerAPI : BaseAPI
    {
        public string Info { get; set; }
        public string ConvertService { get; set; }
        public string DocBuilder { get; set; }
    }
}
