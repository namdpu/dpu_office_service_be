namespace OfficeService.Common
{
    public class Constants
    {
        public static string[] fileTypes = ["csv", "djvu", "doc", "docm", "docx", "dot", "dotm", "dotx", "epub", "fb2", "fodp", "fods", "fodt", "htm", "html", "hwp", "hwpx", "key", "md", "mht", "numbers", "odg", "odp", "ods", "odt", "otp", "ots", "ott", "oxps", "pages", "pdf", "pot", "potm", "potx", "pps", "ppsm", "ppsx", "ppt", "pptm", "pptx", "rtf", "txt", "vsdm", "vsdx", "vssm", "vssx", "vstm", "vstx", "xls", "xlsb", "xlsm", "xlsx", "xlt", "xltm", "xltx", "xml", "xps"];
        public static string[] documentTypes = ["word", "cell", "slide", "pdf", "diagram"];

        public static Enums.DocumentStatus[] StatusSave = [Enums.DocumentStatus.ReadyForSaving, Enums.DocumentStatus.EditingSavedState];
        //
        public const string DATA_NULL = "ERR_DATA_NOT_FOUND";
        public const string ERROR_NOTI = "SYS_ERR";
    }
}
