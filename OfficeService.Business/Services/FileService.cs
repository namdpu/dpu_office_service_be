using DPUStorageService.APIs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeService.Business.IServices;
using OfficeService.Common;
using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;
using OfficeService.DAL.IRepository;
using OfficeService.DAL.Models;
using System;
using System.Text;
using System.Text.Json;

namespace OfficeService.Business.Services
{
    public class FileService : BaseService<DAL.Entities.File>, IFileService
    {
        private readonly IFileRepository _rp;
        private readonly IFileVersionRepository _fileVersionRP;
        private readonly IJWTContext _jWTContext;
        private readonly IUserContext _userContext;
        private readonly IApiStorage _apiStorage;
        private readonly AppSetting _setting;
        private static readonly object _customHeaderLock = new object();
        private readonly CachingService _cachingService;
        public FileService(
            IFileRepository rp,
            IJWTContext jWTContext,
            ILogger<FileService> logger,
            IUserContext userContext,
            IFileVersionRepository fileVersionRP,
            IApiStorage apiStorage,
            IOptions<AppSetting> options,
            CachingService cachingService) : base(rp, logger)
        {
            this._rp = rp;
            _jWTContext = jWTContext;
            _userContext = userContext;
            _fileVersionRP = fileVersionRP;
            _apiStorage = apiStorage;
            _setting = options.Value;
            if (string.IsNullOrEmpty(_apiStorage.Configuration.CustomHeader.GetValueOrDefault(nameof(_setting.InternalKey))))
                lock (_customHeaderLock)
                {
                    if (string.IsNullOrEmpty(_apiStorage.Configuration.CustomHeader.GetValueOrDefault(nameof(_setting.InternalKey))))
                    {
                        _apiStorage.Configuration.CustomHeader.Add(nameof(_setting.InternalKey), _setting.InternalKey);
                    }
                }
            _cachingService = cachingService;
        }

        public async Task<BaseResponse> GetVersionHistory(string key, Guid appId)
        {
            try
            {
                var existFile = await _rp.FindWithIncludesAsync(x => x.AppId == appId && x.FileKey == key, x => x.Include(y => y.FileVersions));
                if (existFile is null)
                    return NotFoundResponse("File not found or not exist!");

                var versionHistories = new VersionHistoryDTO
                {
                    CurrentVersion = existFile.FileVersions
                        .Where(x => x.IsActive && !x.IsDeleted)
                        .OrderBy(x => x.SystemVersion).LastOrDefault()?.Version ?? "Unknown Version",
                    History = existFile.FileVersions
                        .Where(x => x.IsActive && !x.IsDeleted)
                        .OrderBy(x => x.SystemVersion)
                        .Select(x =>
                        {
                            var users = x.Users is not null ? JsonConvert.DeserializeObject<List<string>>(x.Users.RootElement.ToString() ?? "") : new List<string>();
                            return new HistoryDTO
                            {
                                Changes = JsonConvert.DeserializeObject(x.Histotry?.RootElement.GetProperty("Changes").ToString() ?? ""),
                                Created = x.LastSave ?? DateTime.UtcNow,
                                Key = x.Id.ToString(),
                                ServerVersion = x.Histotry?.RootElement.GetProperty("ServerVersion").ToString() ?? string.Empty,
                                User = users?.FirstOrDefault() ?? "Unknown User",
                                Version = x.Version ?? "Unknown Version"
                            };
                        }).ToArray()
                };

                return SuccessResponse(versionHistories, "Get version histories successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when get version history {key} in app {appId}");

                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetHistoryData(Config config, Guid appId)
        {
            try
            {
                var existFile = await _rp.FindWithIncludesAsync(x => x.AppId == appId && x.FileKey == config.Document.Key, x => x.Include(y => y.FileVersions));
                if (existFile is null)
                    return NotFoundResponse("File not found or not exist!");

                var existFileVer = existFile.FileVersions
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .OrderByDescending(x => x.SystemVersion)
                    .FirstOrDefault(x => string.IsNullOrEmpty(config.Document.Version) || config.Document.Version == x.Version);
                if (existFileVer is null)
                    return NotFoundResponse("File version not found or not exist!");

                string key = GenDocKey(existFileVer.Id, false);
                config.Document.Key = key;

                var historyData = new HistoryDataDTO
                {
                    ChangesUrl = existFileVer.ChangesUrl?.Replace("localhost", "host.docker.internal"),
                    FileType = existFile.FileType,
                    Key = key,
                    Url = existFileVer.Url.Replace("localhost", "host.docker.internal"),
                    Version = existFileVer.Version ?? "Unknown Version",
                };

                var accessToken = this.GetAccessTokenHistoryData(historyData);
                if (string.IsNullOrEmpty(accessToken))
                    return BadRequestResponse("Generate token failed!");

                historyData.Token = accessToken;

                return SuccessResponse(historyData, "Get history data successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when get history data  {config.Document.Key} in app {appId}");

                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetToken(Config config, Guid appId)
        {
            try
            {
                if(string.IsNullOrEmpty(config.Document.Url))
                    return BadRequestResponse("File URL is required!");

                var newConfig = config.Adapt<ConfigDTO>();
                var existFile = await _rp.FindWithIncludesAsync(x => x.AppId == appId && x.FileKey == config.Document.Key && x.Type == config.Type && x.DocumentType == config.DocumentType, 
                    x => x.Include(y => y.FileVersions));
                if(existFile is null)
                {
                    if(string.IsNullOrEmpty(config.Document.Url))
                        return BadRequestResponse("File URL is required!");

                    string key = GenDocKey(Guid.NewGuid());
                    existFile = new DAL.Entities.File
                    {
                        AppId = appId,
                        FileKey = config.Document.Key,
                        Size = 0, // check get capacity file
                        Url = config.Document.Url,
                        OriginUrl = config.Document.Url,
                        DocumentType = config.DocumentType,
                        Type = config.Type,
                        CallBackUrl = config.EditorConfig?.CallbackUrl,
                        Key = key,
                        Status = Enums.DocumentStatus.BeingEdited,
                        FileType = config.Document.FileType,
                        FileVersions = new List<DAL.Entities.FileVersion>()
                        {
                            new DAL.Entities.FileVersion
                            {
                                Url = config.Document.Url,
                                Status = Enums.DocumentStatus.BeingEdited,
                                SystemVersion = 1,
                                Key = key,
                                Version = config.Document.Version,
                                Users = string.IsNullOrEmpty(config.Document?.Info?.OwnerId) ? null : JsonDocument.Parse(JsonConvert.SerializeObject(new List<string> { config.Document.Info.OwnerId })),
                            }
                        }
                    };

                    await _rp.Insert(existFile);
                    await _rp.Save();
                    newConfig.Document.Key = existFile.Key;
                }
                else
                {
                    int systemVersion = existFile.FileVersions.Count() + 1;
                    existFile.Url = config.Document.Url;
                    existFile.OriginUrl = config.Document.Url;
                    existFile.CallBackUrl = config.EditorConfig?.CallbackUrl;
                    if(existFile.Status == Enums.DocumentStatus.ReadyForSaving)
                    {
                        existFile.Key = GenDocKey(existFile.Id);
                        existFile.Status = Enums.DocumentStatus.BeingEdited;
                    }    
                    await _rp.Update(existFile);
                    await _rp.Save();
                    newConfig.Document.Key = existFile.Key;

                    if(string.IsNullOrEmpty(config.Document.Version))
                    {
                        existFile.FileVersions.Add(new DAL.Entities.FileVersion
                        {
                            Url = config.Document.Url,
                            Status = Enums.DocumentStatus.BeingEdited,
                            SystemVersion = systemVersion,
                            Key = GenDocKey(Guid.NewGuid()),
                            Version = config.Document.Version
                        });
                    }
                    else
                    {
                        var existFileVer = existFile.FileVersions.FirstOrDefault(x => x.Version == config.Document.Version);
                        if (existFileVer is null)
                        {
                            existFile.FileVersions.Add(new DAL.Entities.FileVersion
                            {
                                Url = config.Document.Url,
                                Status = Enums.DocumentStatus.BeingEdited,
                                SystemVersion = systemVersion,
                                Key = GenDocKey(Guid.NewGuid()),
                                Version = config.Document.Version
                            });
                        }
                        else
                            if (existFile.FileVersions.Max(x => x.SystemVersion) != existFileVer.SystemVersion)
                            {
                                if (existFile.Status == Enums.DocumentStatus.ReadyForSaving)
                                {
                                    existFileVer.Key = GenDocKey(existFileVer.Id);
                                    existFileVer.Status = Enums.DocumentStatus.BeingEdited;
                                    await _fileVersionRP.Update(existFileVer);
                                    await _fileVersionRP.Save();
                                }
                                newConfig.Document.Key = existFileVer.Key;
                            }
                    }
                }

                newConfig.Document.ReferenceData = new ReferenceData
                {
                    FileKey = config.Document!.Key,
                    InstanceId = appId.ToString()
                };
                if(newConfig.EditorConfig is not null)
                    newConfig.EditorConfig.CallbackUrl = "http://host.docker.internal:5126/api/file/Callback";

                var accessToken = this.GetAccessToken(newConfig);
                if(string.IsNullOrEmpty(accessToken))
                    return BadRequestResponse("Generate token failed!");

                newConfig.Token = accessToken;

                return SuccessResponse(newConfig, "Get token view file successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when get token view file {config.Document.Key} in app {appId}");

                return CatchErrorResponse(ex);
            }
        }

        public async Task<object> HandleCallback(CallbackHandlerPayload data)
        {
            try
            {
                if(Constants.StatusSave.Contains(data.Status))
                {
                    if (string.IsNullOrEmpty(data.Url))
                        throw new Exception("File url is null or empty!");

                    var existFile = await _rp.FindWithIncludesAsync(x => x.Key == data.Key, query => query.Include(q => q.FileVersions));
                    if(existFile is not null)
                    {
                        // Trường hợp user nhấn nút lưu trên giao diện và tắt tài liệu thì callback vẫn nhận về giá trị changes, cần kiểm tra xem có trùng nhau không để biết không cần lưu
                        if(data.History?.Changes is not null)
                        {
                            if (existFile.FileVersions.Any(x =>
                            {
                                var changes = JsonConvert.DeserializeObject<HistoryDataChanges[]>(x.Histotry?.RootElement.GetProperty("Changes").ToString() ?? "");
                                if (changes is not null && data.History.Changes.Length == changes.Length && data.History.Changes.All(c => changes.Any(dc => dc.DocumentSha256 == c.DocumentSha256)))
                                    return true;
                                
                                return false;
                            }))
                            {
                                if(data.Status == Enums.DocumentStatus.ReadyForSaving)
                                {
                                    existFile.Status = Enums.DocumentStatus.ReadyForSaving;
                                    await _rp.UpdateWithoutTracking(existFile);
                                    await _rp.Save();
                                }

                                return new
                                {
                                    error = 0
                                };
                            }
                        }

                        int systemVersion = existFile.FileVersions.Count() + 1;
                        existFile.Status = data.Status;
                        existFile.FileVersions.Add(new DAL.Entities.FileVersion
                        {
                            ChangesUrl = data.ChangesUrl,
                            Status = data.Status,
                            Url = data.Url,
                            LastSave = data.LastSave,
                            ForceSaveType = data.ForceSaveType,
                            Histotry = data.History is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.History)) : null,
                            Users = data.Users is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.Users)) : null,
                            Actions = data.Actions is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.Actions)) : null,
                            SystemVersion = systemVersion,
                            RefId = null,
                            Key = GenDocKey(Guid.NewGuid())
                        });
                        await _rp.UpdateWithoutTracking(existFile);
                        await _rp.Save();

                        if (!string.IsNullOrEmpty(existFile.CallBackUrl))
                        {
                            HttpClient client = new HttpClient();
                            var payload = new
                            {
                                key = existFile.FileKey,
                                url = data.Url,
                                userId = data.Users?.Last() ?? string.Empty,
                                systemVersion = systemVersion,
                                status = data.Status
                            };
                            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(existFile.CallBackUrl, stringContent);
                            string? stringData = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                if (!string.IsNullOrEmpty(stringData))
                                {
                                    var dataObj = JsonConvert.DeserializeObject<dynamic>(stringData);
                                    if (dataObj is not null)
                                    {
                                        var fileVer = existFile.FileVersions.FirstOrDefault(x => x.SystemVersion == systemVersion);
                                        if (fileVer is not null)
                                        {
                                            var versionProp = dataObj["version"];
                                            if(versionProp is not null)
                                            {
                                                fileVer.Version = versionProp.ToString();
                                                await _fileVersionRP.UpdateWithoutTracking(fileVer);
                                                await _fileVersionRP.Save();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(stringData))
                                {
                                    var dataObj = JsonConvert.DeserializeObject<object>(stringData);
                                    if (dataObj is not null)
                                    {
                                        return dataObj;
                                    }
                                }
                                return new
                                {
                                    error = -1,
                                    message = "Internal server error from callback URL"
                                };
                            }
                        }

                        return new
                        {
                            error = 0
                        };
                    }

                    var existFileVer = await _fileVersionRP.FindWithIncludesAsync(x => x.Key == data.Key, query => query.Include(q => q.File).ThenInclude(f => f.FileVersions));
                    if (existFileVer is not null)
                    {

                        // Trường hợp user nhấn nút lưu trên giao diện và tắt tài liệu thì callback vẫn nhận về giá trị changes, cần kiểm tra xem có trùng nhau không để biết không cần lưu
                        if (data.History?.Changes is not null)
                        {
                            if (existFileVer.File.FileVersions.Any(x =>
                            {
                                var changes = JsonConvert.DeserializeObject<HistoryDataChanges[]>(x.Histotry?.RootElement.GetProperty("Changes").ToString() ?? "");
                                if (changes is not null && data.History.Changes.Length == changes.Length && data.History.Changes.All(c => changes.Any(dc => dc.DocumentSha256 == c.DocumentSha256)))
                                    return true;

                                return false;
                            }))
                            {
                                if (data.Status == Enums.DocumentStatus.ReadyForSaving)
                                {
                                    existFileVer.Status = Enums.DocumentStatus.ReadyForSaving;
                                    await _fileVersionRP.UpdateWithoutTracking(existFileVer);
                                    await _fileVersionRP.Save();
                                }

                                return new
                                {
                                    error = 0
                                };
                            }
                        }
                        int systemVersion = existFileVer.File.FileVersions.Count() + 1;
                        var newFileVer = new DAL.Entities.FileVersion
                        {
                            ChangesUrl = data.ChangesUrl,
                            Status = data.Status,
                            Url = data.Url,
                            LastSave = data.LastSave,
                            ForceSaveType = data.ForceSaveType,
                            Histotry = data.History is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.History)) : null,
                            Users = data.Users is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.Users)) : null,
                            Actions = data.Actions is not null ? JsonDocument.Parse(JsonConvert.SerializeObject(data.Actions)) : null,
                            SystemVersion = systemVersion,
                            RefId = existFileVer.Id,
                            Key = GenDocKey(Guid.NewGuid())
                        };
                        existFileVer.File.FileVersions.Add(newFileVer);
                        await _rp.UpdateWithoutTracking(existFileVer.File);
                        await _rp.Save();

                        if (!string.IsNullOrEmpty(existFileVer.File.CallBackUrl))
                        {
                            HttpClient client = new HttpClient();
                            var payload = new
                            {
                                key = existFileVer.File.FileKey,
                                url = existFileVer.Url,
                                userId = data.Users?.Last() ?? string.Empty,
                                systemVersion = systemVersion,
                                status = data.Status
                            };
                            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(existFileVer.File.CallBackUrl, stringContent);
                            string? stringData = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                if (!string.IsNullOrEmpty(stringData))
                                {
                                    var dataObj = JsonConvert.DeserializeObject<dynamic>(stringData);
                                    var systemVersionProp = dataObj?["systemVersion"];
                                    if (dataObj is not null && systemVersionProp is not null)
                                    {
                                        if (existFileVer.SystemVersion == systemVersionProp)
                                        {
                                            var versionProp = dataObj["version"];
                                            if (versionProp is not null)
                                            {
                                                existFileVer.Version = versionProp.ToString();
                                                await _fileVersionRP.UpdateWithoutTracking(existFileVer);
                                                await _fileVersionRP.Save();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(stringData))
                                {
                                    var dataObj = JsonConvert.DeserializeObject<object>(stringData);
                                    if (dataObj is not null)
                                    {
                                        return dataObj;
                                    }
                                }
                                return new
                                {
                                    error = -1,
                                    message = "Internal server error from callback URL"
                                };
                            }
                        }
                    }
                    else
                        return new
                        {
                            error = -1,
                            message = "File key not found"
                        };
                }

                return new
                {
                    error = 0
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handle callback");

                return new
                {
                    error = -1,
                    message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> RestoreVersion(string key, string version, string newVersion, string userId, Guid appId)
        {
            try
            {
                var existNewFileVer = await _fileVersionRP.Queryable().FirstOrDefaultAsync(x =>
                    x.File.AppId == appId
                    && x.File.FileKey == key
                    && x.Version == newVersion
                    && x.File.IsActive && !x.File.IsDeleted
                    && x.IsActive && !x.IsDeleted);
                if (existNewFileVer is not null)
                    return BadRequestResponse("New version is exist, please choose another version!");

                var existFileVer = await _fileVersionRP.FindWithIncludesAsync(x =>
                    x.File.AppId == appId
                    && x.File.FileKey == key
                    && x.Version == version
                    && x.File.IsActive && !x.File.IsDeleted
                    && x.IsActive && !x.IsDeleted,
                    query => query.Include(fv => fv.File).ThenInclude(f => f.FileVersions));
                if (existFileVer is null)
                    return NotFoundResponse("File version not found or not exist!");

                existNewFileVer = new DAL.Entities.FileVersion
                {
                    FileId = existFileVer.FileId,
                    SystemVersion = existFileVer.File.FileVersions.Count() + 1,
                    Url = existFileVer.Url,
                    Status = Enums.DocumentStatus.BeingEdited,
                    RefId = existFileVer.Id,
                    Key = GenDocKey(Guid.NewGuid()),
                    Version = newVersion,
                    Users = JsonDocument.Parse(JsonConvert.SerializeObject(new List<string> { userId }))
                };
                await _fileVersionRP.Insert(existNewFileVer);
                await _fileVersionRP.Save();

                return await this.GetVersionHistory(key, appId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when restore file version {key} in app {appId}");

                return CatchErrorResponse(ex);
            }
        }

        private string GenDocKey(Guid id, bool isAddTime = true)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = utcNow - epoch;
            string plainText = $"{id}-{(int)timeSpan.TotalSeconds}";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            return encoded;
            //// Decode lại
            //string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            //Console.WriteLine("Decoded: " + decoded);
        }

        private string? GetAccessToken(ConfigDTO config)
        {
            Dictionary<string, string> claims = new();
            var documentJson = JsonConvert.SerializeObject(config.Document, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            claims.Add("document", documentJson);

            if(config.EditorConfig is not null)
            {
                var editorConfigJson = JsonConvert.SerializeObject(config.EditorConfig, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                claims.Add("editorConfig", editorConfigJson);
            }    

            var token = _jWTContext.GenerateToken(new RequestToken()
            {
                ExpiresIn = 84600 // 24 hours
            }, claims);

            return token?.AccessToken;
        }

        private string? GetAccessTokenHistoryData(HistoryDataDTO historyDataDTO)
        {
            Dictionary<string, string> claims = new();
            var documentJson = JsonConvert.SerializeObject(historyDataDTO, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            foreach(var prop in historyDataDTO.GetType().GetProperties())
            {
                var value = prop.GetValue(historyDataDTO);
                if(value is not null)
                    claims.Add(char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1), value.ToString() ?? string.Empty);
            }

            var token = _jWTContext.GenerateToken(new RequestToken()
            {
                ExpiresIn = 84600 // 24 hours
            }, claims);

            return token?.AccessToken;
        }

        public async Task<BaseResponse> DeleteVersion(string key, string version, Guid appId)
        {
            try
            {
                var existFileVer = await _fileVersionRP.FindWithIncludesAsync(x =>
                    x.File.AppId == appId
                    && x.File.FileKey == key
                    && x.Version == version
                    && x.File.IsActive && !x.File.IsDeleted
                    && x.IsActive && !x.IsDeleted,
                    query => query.Include(fv => fv.File).ThenInclude(f => f.FileVersions));
                if (existFileVer is null)
                    return NotFoundResponse("File version not found or not exist!");

                await _fileVersionRP.SoftDelete(existFileVer);
                await _fileVersionRP.Save();

                string prefix = $"officedata/{appId}/{existFileVer.File.Id}/v{existFileVer.SystemVersion}";

                await this.JobDeletePrefixInCloud(appId, prefix);

                return SuccessResponse(new
                {
                    key = key,
                    version = version
                }, "Delete file version successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when restore file version {key} in app {appId}");

                return CatchErrorResponse(ex);
            }
        }

        private async Task JobDeletePrefixInCloud(Guid appId, string prefix, string? key = default)
        {
            try
            {
                var application = await _cachingService.GetApplication(appId);
                string bucketName = application.BucketName;

                await _apiStorage.JobDeletePrefixInCloud(bucketName, prefix, key ?? string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when delete prefix {prefix} in cloud with key {key} in app {appId}");
                throw;
            }
        }
    }
}
