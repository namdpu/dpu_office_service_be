using OfficeService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json;
using static OfficeService.Common.Enums;

namespace OfficeService.DAL.Entities
{
    /// <summary>
    /// File version
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("file_version", Schema = "public")]
    public class FileVersion : AuditEntity
    {
        /// <summary>
        /// File Id
        /// </summary>
        [DataMember]
        [Column("file_id")]
        [Required]
        public Guid FileId { get; set; }

        /// <summary>
        /// File Key
        /// </summary>
        [DataMember]
        [Column("key")]
        [MaxLength(64)]
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Client version of file
        /// </summary>
        [DataMember]
        [Column("version")]
        public string? Version { get; set; }

        /// <summary>
        /// System version of file
        /// </summary>
        [DataMember]
        [Column("system_version")]
        [Required]
        public int SystemVersion { get; set; }

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
        /// Changes Url of file
        /// </summary>
        [DataMember]
        [Column("changes_url")]
        [MaxLength(1024)]
        public string? ChangesUrl { get; set; }

        /// <summary>
        /// Status of file
        /// </summary>
        [DataMember]
        [Column("status")]
        [Required]
        public Enums.DocumentStatus Status { get; set; }

        /// <summary>
        /// Last Save
        /// </summary>
        [DataMember]
        [Column("last_save")]
        public DateTime? LastSave { get; set; }

        /// <summary>
        /// Force Save Type
        /// </summary>
        [DataMember]
        [Column("force_save_type")]
        public ForceSaveType? ForceSaveType { get; set; }

        /// <summary>
        /// History changes of file
        /// </summary>
        [DataMember]
        [Column("histotry")]
        public JsonDocument? Histotry { get; set; }

        /// <summary>
        /// History changes of file
        /// </summary>
        [DataMember]
        [Column("users")]
        public JsonDocument? Users { get; set; }

        /// <summary>
        /// History changes of file
        /// </summary>
        [DataMember]
        [Column("actions")]
        public JsonDocument? Actions { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [DataMember]
        [Column("ref_id")]
        public Guid? RefId { get; set; }

        /// <summary>
        /// Show whether the origin file is already on cloud or not
        /// </summary>
        [DataMember]
        [Column("synced_file")]
        public bool SyncedFile { get; set; }

        /// <summary>
        /// Show whether the file changes.zip is already on cloud or not
        /// </summary>
        [DataMember]
        [Column("synced_changes")]
        public bool SyncedChanges { get; set; }

        public virtual Entities.File File { get; set; }
    }
}
