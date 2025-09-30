using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeService.DAL.DTOs.Requests
{
    public class SaveAsRequest
    {
        public string? Key { get; set; }
        public string? Version { get; set; }
        public string? Url { get; set; }
        public string FileType { get; set; }
        public string OutputType { get; set; }
        public string? Title { get; set; }
    }
}
