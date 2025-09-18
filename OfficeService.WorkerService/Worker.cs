using Microsoft.Extensions.Options;
using OfficeService.Business.IServices;
using OfficeService.DAL.Models;

namespace OfficeService.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSetting _setting;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IOptions<AppSetting> options, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _setting = options.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                     _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        // Resolve Transient service mỗi lần loop
                        var _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                        await _fileService.HandleSyncData();
                    }
                    _logger.LogInformation("Worker finished at: {time}", DateTime.UtcNow);
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
