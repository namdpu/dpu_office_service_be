using OfficeService.DAL.DTOs.Requests;
using System.Text.Json.Serialization;

namespace OfficeService.DAL.DTOs.Responses
{
    public class ResponseToken
    {
        public string AccessToken { get; set; }
        public int? ExpiresIn { get; set; }
    }

    public class ConfigDTO
    {
        /// <summary>
        /// Defines the document type to be opened (word, cell, slide, pdf, diagram)
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Defines the platform type used to access the document.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Configuration for the current document.
        /// </summary>
        public DocumentDTO Document { get; set; }

        /// <summary>
        /// Configuration for the editor.
        /// </summary>
        public EditorConfig? EditorConfig { get; set; }

        /// <summary>
        /// Defines the encrypted signature added to the ONLYOFFICE Docs config in the form of a token.
        /// </summary>
        public string Token { get; set; }
    }

    public class DocumentDTO : Document
    {
        public ReferenceData ReferenceData { get; set; }
    }

    public class ReferenceData
    {
        public string FileKey { get; set; }
        public string InstanceId { get; set; }
    }
}
