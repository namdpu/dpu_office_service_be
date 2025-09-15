using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeService.DAL.IRepository;
using OfficeService.DAL.Models;

namespace OfficeService.DAL.Repository
{
    public class FileRepository : BaseRepository<Entities.File>, IFileRepository
    {
        public FileRepository(IUserContext userContext, DataBaseContext context, IOptions<AppSetting> options, ILogger<FileRepository> logger)
            : base(userContext, context, options, logger)
        {
        }
    }
}
