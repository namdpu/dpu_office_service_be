using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeService.DAL.IRepository;
using OfficeService.DAL.Models;

namespace OfficeService.DAL.Repository
{
    public class FileVersionRepository : BaseRepository<Entities.FileVersion>, IFileVersionRepository
    {
        public FileVersionRepository(IUserContext userContext, DataBaseContext context, IOptions<AppSetting> options, ILogger<FileVersionRepository> logger)
            : base(userContext, context, options, logger)
        {
        }
    }
}
