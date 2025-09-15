using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;

namespace OfficeService.Business.IServices
{
    public interface IFileService : IBaseService<DAL.Entities.File>
    {
        Task<BaseResponse> GetToken(Config config, Guid appId);
        Task<BaseResponse> GetVersionHistory(string key, Guid appId);
        Task<BaseResponse> GetHistoryData(Config config, Guid appId);
        Task<BaseResponse> RestoreVersion(string key, string version, string newVersion, string userId, Guid appId);
        Task<BaseResponse> DeleteVersion(string key, string version, Guid appId);
        Task<object> HandleCallback(CallbackHandlerPayload data);
    }
}
