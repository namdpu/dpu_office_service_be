using Microsoft.AspNetCore.Mvc;
using OfficeService.DAL.DTOs.Responses;
using System.Security.Claims;

namespace OfficeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public IConfiguration _configuration;
        protected readonly ILogger _logger;

        public BaseController(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private ClaimsPrincipal _currentUser
        {
            get { return HttpContext.User; }
        }

        protected Guid LoginedUserId
        {
            get
            {
                if (_currentUser.Identity.IsAuthenticated)
                {
                    return Guid.Parse(this.HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                }
                return Guid.Empty;
            }
        }


        protected ObjectResult SuccessResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status200OK, response);
        }

        protected ObjectResult BadRequestResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status400BadRequest, response);
        }

        protected ObjectResult NotFoundResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status404NotFound, response);
        }

        protected ObjectResult CatchErrorResponse(Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);

            return StatusCode(StatusCodes.Status500InternalServerError, createModel(null, e.Message, 500));
        }

        protected ObjectResult ReturnData(BaseResponse response)
        {
            switch (response.StatusCode)
            {
                case 200:
                    {
                        return SuccessResponse(response);
                    };
                case 400:
                    {
                        return BadRequestResponse(response);
                    };
                case 404:
                    {
                        return NotFoundResponse(response);
                    };
                default: return null;
            }
        }

        protected BaseResponse createModel(object data = null, string message = "", int statusCode = 200)
        {
            return new BaseResponse()
            {
                StatusCode = statusCode,
                Data = data,
                Message = message,
            };
        }
    }

}
