namespace OfficeService.DAL.Models
{
    public class ConfigAPI
    {
        public AuthAPI AuthAPI { get; set; }
    }

    public abstract class BaseAPI
    {
        public string Endpoint { get; set; }
    }
    public class AuthAPI : BaseAPI
    {
        public string GetApplication { get; set; }
    }
}
