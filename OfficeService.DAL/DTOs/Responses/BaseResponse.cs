namespace OfficeService.DAL.DTOs.Responses
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 200;
        public object Message { get; set; } = "";
        public object? Data { get; set; }
        public string ErrorCode { get; set; } = "";
    }

    //public class AuditResponse
    //{
    //    public UserInfo? CreateBy { get; set; }
    //    public UserInfo? UpdateBy { get; set; }
    //    [JsonIgnore]
    //    public Guid? CreatedUserId { get; set; }
    //    [JsonIgnore]
    //    public Guid? UpdatedUserId { get; set; }
    //}
}
