using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeService.DAL.DTOs.Responses;
using OfficeService.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeService.Business.Services
{
    public class CachingService
    {
        private readonly IMemoryCache _cache;
        private readonly ConfigAPI _configAPI;
        private readonly AppSetting _appSetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public CachingService(IMemoryCache cache, IOptions<ConfigAPI> options, IOptions<AppSetting> options1, IHttpContextAccessor httpContextAccessor, ILogger<CachingService> logger)
        {
            _cache = cache;
            _configAPI = options.Value;
            _appSetting = options1.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApplicationDTO> GetApplication(Guid addId)
        {
            var application = _cache.Get<ApplicationDTO>(addId.ToString());
            if (application is null)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add(nameof(_appSetting.InternalKey), _appSetting.InternalKey);
                var response = await client.GetAsync($"{_configAPI.AuthAPI.Endpoint}/{_configAPI.AuthAPI.GetApplication}/{addId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        var result = JsonConvert.DeserializeObject<BaseResponse>(content);
                        if (result is not null && result.StatusCode == 200 && result.Data is not null)
                        {
                            application = JsonConvert.DeserializeObject<ApplicationDTO>(result.Data.ToString());
                            if (application is not null)
                            {
                                _cache.Set(addId.ToString(), application);
                            }
                        }
                    }
                }
            }
            if (application is null)
            {
                _logger.LogError($"Can not find application {addId}");
                throw new Exception("Error system, please contact administrator");
            }

            return application;
        }
    }
}
