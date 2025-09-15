using OfficeService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OfficeService.Common.Enums;

namespace OfficeService.DAL.DTOs.Requests
{
    /// <summary>
    /// Represents the payload sent by ONLYOFFICE Document Server to the callback handler URL.
    /// </summary>
    public class CallbackHandlerPayload
    {
        /// <summary>
        /// Array of actions performed by users (connect, disconnect, forcesave).
        /// </summary>
        public List<ActionInfo>? Actions { get; set; }

        /// <summary>
        /// Contains the document's change history; present when status is 2 or 3.
        /// </summary>
        public HistoryData? History { get; set; }

        /// <summary>
        /// URL to the file containing the document edit history changes; for statuses 2, 3, 6, or 7.
        /// </summary>
        public string? ChangesUrl { get; set; }

        /// <summary>
        /// File's extension/type (e.g., "docx").
        /// </summary>
        public string? FileType { get; set; }

        /// <summary>
        /// Type of initiator of 'force save' (0–3); present when status is 6 or 7.
        /// </summary>
        public ForceSaveType? ForceSaveType { get; set; }

        /// <summary>
        /// For status=6 and ForceSaveType=3, contains the URL to submitted forms data.
        /// </summary>
        public FormsDataUrlInfo? FormsDataUrl { get; set; }

        /// <summary>
        /// The unique key assigned to the edited document.
        /// </summary>
        public string Key { get; set; } = default!;

        /// <summary>
        /// Document editing status (1–7), indicating the current state as defined by ONLYOFFICE.
        /// </summary>
        public Enums.DocumentStatus Status { get; set; }

        /// <summary>
        /// URL to the current edited document for download; present when status is 2, 3, 6, or 7.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Custom user data that may be sent alongside force-save or info commands.
        /// </summary>
        public string? UserData { get; set; }

        /// <summary>
        /// List of user IDs who opened or last edited the document.
        /// </summary>
        public List<string>? Users { get; set; }

        /// <summary>
        /// Last time save file
        /// </summary>
        public DateTime? LastSave { get; set; }
    }

    /// <summary>
    /// Represents an entry in the actions array.
    /// </summary>
    public class ActionInfo
    {
        /// <summary>
        /// Action type: 0 = disconnect, 1 = connect, 2 = force save button click.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Identifier of the user who performed the action.
        /// </summary>
        public string? UserId { get; set; }
    }

    /// <summary>
    /// Contains document change history. Present when status is 2 or 3.
    /// </summary>
    public class HistoryData
    {
        /// <summary>
        /// Document changes (to be used with refreshHistory in the editor).
        /// </summary>
        public HistoryDataChanges[]? Changes { get; set; }

        /// <summary>
        /// Server version number for the document history.
        /// </summary>
        public string? ServerVersion { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HistoryDataChanges
    {
        public object User { get; set; }
        public DateTime Created { get; set; }
        public string DocumentSha256 { get; set; }
    }

    /// <summary>
    /// Contains form submission data info when submitting forms (status=6, ForceSaveType=3).
    /// </summary>
    public class FormsDataUrlInfo
    {
        /// <summary>
        /// URL pointing to the JSON file with the submitted form data.
        /// </summary>
        public string? Url { get; set; }
    }
}
