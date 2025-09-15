using OfficeService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace OfficeService.DAL.Entities
{
    /// <summary>
    /// File
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("file", Schema = "public")]
    public class File : AuditEntity
    {
        /// <summary>
        /// Application Id
        /// </summary>
        [DataMember]
        [Column("app_id")]
        [Required]
        public Guid AppId { get; set; }

        /// <summary>
        /// File Key
        /// </summary>
        [DataMember]
        [Column("key")]
        [MaxLength(64)]
        [Required]
        public string Key { get; set; }
        
        /// <summary>
        /// File Key
        /// </summary>
        [DataMember]
        [Column("file_key")]
        [MaxLength(1024)]
        [Required]
        public string FileKey { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        [DataMember]
        [Column("size")]
        [Required]
        public long Size { get; set; }

        /// <summary>
        /// Url of file
        /// </summary>
        [DataMember]
        [Column("url")]
        [MaxLength(1024)]
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// Origin Url of file
        /// </summary>
        [DataMember]
        [Column("origin_url")]
        [MaxLength(1024)]
        [Required]
        public string OriginUrl { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        [DataMember]
        [Column("document_type")]
        [MaxLength(50)]
        [Required]
        public string DocumentType { get; set; }

        /// <summary>
        /// Type of file
        /// </summary>
        [DataMember]
        [Column("type")]
        [MaxLength(50)]
        public string? Type { get; set; }

        /// <summary>
        /// Callback url
        /// </summary>
        [DataMember]
        [Column("callback_url")]
        [MaxLength(1024)]
        public string? CallBackUrl { get; set; }

        /// <summary>
        /// Status of file
        /// </summary>
        [DataMember]
        [Column("status")]
        [Required]
        public Enums.DocumentStatus Status { get; set; }

        /// <summary>
        /// File Type (ex: docx, doc, pdf, xlsx...)
        /// </summary>
        [DataMember]
        [Column("file_type")]
        [MaxLength(50)]
        [Required]
        public string FileType { get; set; }

        public virtual ICollection<FileVersion> FileVersions { get; set; } = new List<FileVersion>();
    }
}
