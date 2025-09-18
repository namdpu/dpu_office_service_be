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
builder.Services.AddSingleton<IJWTContext, JWTContext>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IApiStorage, ApiStorage>();
builder.Services.AddSingleton<CachingService>();
// DI repository
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IFileVersionRepository, FileVersionRepository>();

// add log
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.UseNLog();
logger.Info($"Application Starting Up At {DateTime.UtcNow}");

builder.Services.AddMemoryCache();
var host = builder.Build();

host.Run();
