using OfficeService.DAL.DTOs.Requests;

namespace OfficeService.DAL.DTOs.Responses
{
    public class VersionHistoryDTO
    {
        public string CurrentVersion { get; set; }
        public HistoryDTO[] History { get; set; } = Array.Empty<HistoryDTO>();
    }

    public class HistoryDTO
    {
        public HistoryDataChanges[]? Changes { get; set; }
        public DateTime Created { get; set; }
        public string Key { get; set; }
        public string ServerVersion { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
    }

    public class HistoryDataDTO
    {
        public string? ChangesUrl { get; set; }
        public string? Error { get; set; }
        public string? FileType { get; set; }
        public string Key { get; set; }
        public PreviousData? Previous { get; set; }
        public string? Token { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
    }

    public class PreviousData
    {
        public string? FileType { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
    }
}
