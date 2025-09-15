using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;

namespace OfficeService.Business.IServices
{
    public interface IJWTContext
    {
        ResponseToken GenerateToken(RequestToken request, Dictionary<string, string> claims);
    }
}
