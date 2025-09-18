using DPUStorageService.APIs;
using DPUStorageService.Client;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using OfficeService.Business.IServices;
using OfficeService.Business.Services;
using OfficeService.DAL;
using OfficeService.DAL.IRepository;
using OfficeService.DAL.Models;
using OfficeService.DAL.Repository;
using OfficeService.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSetting>(appSettings);
var config = new Config();
builder.Configuration.GetSection("Config").Bind(config);
if (config is not null)
{
    builder.Services.AddSingleton(config);
}
// Add connection database
builder.Services.AddDbContext<DataBaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});
// Add services to the container.
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IApiStorage, ApiStorage>();
// DI repository
builder.Services.AddTransient<IFileRepository, FileRepository>();

// add log
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.UseNLog();
logger.Info($"Application Starting Up At {DateTime.UtcNow}");

var host = builder.Build();

host.Run();
