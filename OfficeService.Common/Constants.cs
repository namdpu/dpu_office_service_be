using static OfficeService.Common.Enums;

namespace OfficeService.Common
{
    public class Constants
    {
        public static string[] fileTypes = ["doc", "docm", "docx", "dot", "dotm", "dotx", "epub", "fb2", "fodt", "htm", "html", "md", "hwp", "hwpx", "mht", "mhtml", "odt", "ott", "pages", "rtf", "stw", "sxw", "txt", "wps", "wpt", "xml", "csv", "et", "ett", "fods", "numbers", "ods", "ots", "sxc", "xls", "xlsb", "xlsm", "xlsx", "xlt", "xltm", "xltx", "dps", "dpt", "fodp", "key", "odg", "odp", "otp", "pot", "potm", "potx", "pps", "ppsm", "ppsx", "ppt", "pptm", "pptx", "sxi", "djvu", "oxps", "pdf", "xps", "vsdm", "vsdx", "vssm", "vssx", "vstm", "vstx"];
        public static string[] documentTypes = ["word", "cell", "slide", "pdf", "diagram"];

        public static Enums.DocumentStatus[] StatusSave = [Enums.DocumentStatus.ReadyForSaving, Enums.DocumentStatus.EditingSavedState];
        //
        public const string DATA_NULL = "ERR_DATA_NOT_FOUND";
        public const string ERROR_NOTI = "SYS_ERR";

        //
        public static string GetFileExtensionFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url);

                // Lấy phần path, ví dụ: /bimnext/.../file.docx
                string path = uri.AbsolutePath;

                // Lấy extension (.docx, .pdf, ...)
                string extension = Path.GetExtension(path);

                return extension.TrimStart('.');
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when get file extension from URL: " + ex.Message);
                return string.Empty;
            }
        }

        public static string GetFileType(FileType fileType)
        {
            switch(fileType)
            {
                case FileType.Word:
                    return "docx";
                case FileType.Pdf:
                    return "pdf";
                default:
                    return "word";
            }
        }
    }
}
