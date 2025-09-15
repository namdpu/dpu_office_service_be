using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
