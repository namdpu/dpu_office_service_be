using Microsoft.AspNetCore.Http;
using OfficeService.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeService.DAL.DTOs.Requests
{
    public class GetKeyInFileRequest
    {
        [Required]
        public Enums.FileType InputFile { get; set; }
        public string? Url { get; set; }
        public IFormFile? File { get; set; }
    }

    public class FillDataToFileReq : GetKeyInFileRequest
    {
        [Required]
        public string Data { get; set; }
        [Required]
        public Enums.FileType OutputFile { get; set; }
    }
}
