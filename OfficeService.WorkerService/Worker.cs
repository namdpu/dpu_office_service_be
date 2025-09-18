using Microsoft.Extensions.Options;
using OfficeService.Business.IServices;
using OfficeService.DAL.Models;

namespace OfficeService.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSetting _setting;
        private readonly IFileService _fileService;

        public Worker(ILogger<Worker> logger, IOptions<AppSetting> options, IFileService fileService)
        {
            _logger = logger;
            _setting = options.Value;
            _fileService = fileService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _fileService.HandleSyncData();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error excute batchjob");
                }
                await Task.Delay(_setting.JobInterval, stoppingToken);
            }
        }
    }
}
