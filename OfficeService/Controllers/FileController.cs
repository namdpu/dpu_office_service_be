
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeService.Business.IServices;
using OfficeService.Common;
using OfficeService.Controllers;
using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Net.Http;
using System.Security.Claims;

namespace WebGisBE.Controllers
{
    [Authorize]
    public class FileController : BaseController
    {
        private readonly IFileService _fileService;
        public FileController(IConfiguration configuration, IFileService fileService, ILogger<FileController> logger) : base(configuration, logger)
        {
            _fileService = fileService;
        }

        [SwaggerOperation("Lấy token view file")]
        [HttpPost]
        public async Task<BaseResponse> GetToken(Config config)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.GetToken(config, Guid.Parse(appId));
        }

        [SwaggerOperation("Lấy thông tin file version")]
        [HttpGet]
        public async Task<BaseResponse> GetVersionHistory(string key)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.GetVersionHistory(key, Guid.Parse(appId));
        }

        [SwaggerOperation("Lấy dữ liệu của version")]
        [HttpPost]
        public async Task<BaseResponse> GetHistoryData(Config config)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.GetHistoryData(config, Guid.Parse(appId));
        }

        [SwaggerOperation("Restore version")]
        [HttpPost]
        public async Task<BaseResponse> RestoreVersion(RestoreVersionReq request)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.RestoreVersion(request.key, request.version, request.newVersion, request.userId, Guid.Parse(appId));
        }

        [SwaggerOperation("Restore version")]
        [HttpDelete]
        public async Task<BaseResponse> DeleteVersion(string key, string version)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.DeleteVersion(key, version, Guid.Parse(appId));
        }

        [SwaggerOperation("Restore version")]
        [HttpPost]
        public async Task<BaseResponse> SaveAs(SaveAsRequest request)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = Constants.DATA_NULL
                };
            }
            return await _fileService.SaveAs(request, Guid.Parse(appId));
        }

        [SwaggerOperation("FillDataToFile")]
        [HttpPost]
        public async Task<IActionResult> FillDataToFile([FromForm] FillDataToFileReq request)
        {
            var appId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appId))
            {
                return Unauthorized();
            }
            var data = await _fileService.FillDataToFile(request, Guid.Parse(appId));
            if(data.StatusCode == 200 && data.Data != null)
            {
                var stream = await DownloadFile(data.Data.ToString() ?? "");
                if (stream == null)
                {
                    return NotFound(new { error = "File not found" });
                }

                string outputType = Constants.GetFileType(request.OutputFile);

                return File(stream, "application/octet-stream", $"filled-data.{outputType}");
            }

            return Ok(data);
        }

        [SwaggerOperation("Callback")]
        [HttpPost]
        // NEED TO IMPLEMENT: Token for outbox
        [AllowAnonymous]
        public async Task<IActionResult> Callback(object data)
        {
            try
            {
                var request = Request;
                var payload = JsonConvert.DeserializeObject<CallbackHandlerPayload>(data.ToString());
                if (payload == null)
                {
                    return Ok(new { error = "Invalid payload" });
                }
                var error = await _fileService.HandleCallback(payload);

                return Ok(error);
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message
                });
            }
        }

        [SwaggerOperation("TestLog")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestLog(string? log)
        {
            _logger.LogInformation("Test log: {log}", log);

            return Ok("");
        }

        private async Task<Stream?> DownloadFile(string url)
        {
            try
            {
                using var client = new  HttpClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return stream;
                }
                else
                {
                    System.Console.WriteLine($"Error downloading file: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error downloading file: {ex.Message}");
                return null;
            }
        }

        public record RestoreVersionReq(string key, string version, string newVersion, string userId);
    }
}
