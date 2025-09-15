using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeService.DAL.DTOs.Responses
{
    public class ApplicationDTO
    {
        public Guid Id { get; set; }
        public ClientCredentials ClientCredentials { get; set; }
        public string BucketName { get; set; }
    }

    public class ClientCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
