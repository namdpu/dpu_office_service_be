using OfficeService.DAL.Models;

namespace OfficeService.DAL.IRepository
{
    public interface IUserContext
    {
        UserContextInfo UserInfo { get; }
    }
}
