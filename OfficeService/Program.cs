using DPUStorageService.APIs;
using DPUStorageService.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeService.Business.IServices;
using OfficeService.Business.Services;
using OfficeService.DAL;
using OfficeService.DAL.IRepository;
using OfficeService.DAL.Models;
using OfficeService.DAL.Repository;
using System.Text;

string policyName = "DPUOffice";
var builder = WebApplication.CreateBuilder(args);
var secretKey = Encoding.ASCII
    .GetBytes(builder.Configuration["AuthSetting:SecretKey"] ?? "DlhXHPrSJgIzqZzhK0nRrVPuOo4nhzVF");
var authority = builder.Configuration["AuthSetting:Authority"] ?? "https://localhost:7153";
var appSettings = builder.Configuration.GetSection("appSettings");
builder.Services.Configure<AppSetting>(appSettings);
var configAPI = builder.Configuration.GetSection("ConfigAPI");
builder.Services.Configure<ConfigAPI>(configAPI);
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

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
}); ;
// jwt
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = authority;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                        Enter 'Bearer' [space] and then your token in the text input below.
                        \r\n\r\nExample: '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
          {
            {
              new OpenApiSecurityScheme
              {
                Reference = new OpenApiReference
                  {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                  },
                  Scheme = "oauth2",
                  Name = "Bearer",
                  In = ParameterLocation.Header,
              },
            new List<string>()
        }
            });
    options.CustomSchemaIds(type => type.FullName?.ToString().Replace("+", "."));
    options.EnableAnnotations();
});
// add policy
string[]? origins = builder.Configuration.GetSection("Origins").Get<string[]>();
builder.Services.AddCors(options => options.AddPolicy(policyName,
    p => p.WithOrigins(origins ?? [])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
          ));

builder.Services.AddHttpContextAccessor();
// DI service
builder.Services.AddSingleton<IJWTContext, JWTContext>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IApiStorage, ApiStorage>();
builder.Services.AddSingleton<CachingService>();
// DI repository
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IFileVersionRepository, FileVersionRepository>();

builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors(policyName);

app.MapHealthChecks("/healthz");

app.Run();
