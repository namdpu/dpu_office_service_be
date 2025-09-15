
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeService.Business.IServices;
using OfficeService.Common;
using OfficeService.Controllers;
using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public record RestoreVersionReq(string key, string version, string newVersion, string userId);
    }
}
