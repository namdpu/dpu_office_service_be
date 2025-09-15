namespace OfficeService.Common
{
    public class Enums
    {
        public enum FileType
        {
            Unknown = -1,
            Word,
            Pdf,
        }

        public enum DocumentStatus
        {
            Unknown = 0,
            /// <summary>
            /// Document is being edited.
            /// </summary>
            BeingEdited = 1,

            /// <summary>
            /// Document is ready for saving.
            /// </summary>
            ReadyForSaving = 2,

            /// <summary>
            /// A document saving error has occurred.
            /// </summary>
            SavingError = 3,

            /// <summary>
            /// Document is closed with no changes.
            /// </summary>
            ClosedWithNoChanges = 4,

            /// <summary>
            /// Document is being edited, but the current document state is saved.
            /// </summary>
            EditingSavedState = 6,

            /// <summary>
            /// An error has occurred while force saving the document.
            /// </summary>
            ForceSavingError = 7
        }

        /// <summary>
        /// Defines the type of initiator when a force saving request is performed.
        /// </summary>
        public enum ForceSaveType
        {
            /// <summary>
            /// The force saving request is performed to the command service.
            /// </summary>
            CommandService = 0,

            /// <summary>
            /// The force saving request is performed each time the saving is done (e.g., the Save button is clicked).
            /// This is only available when the forcesave option is set to true.
            /// </summary>
            OnSave = 1,

            /// <summary>
            /// The force saving request is performed by a timer with settings from the server config.
            /// </summary>
            Timer = 2,

            /// <summary>
            /// The force saving request is performed each time the form is submitted (e.g., the "Complete & Submit" button is clicked).
            /// </summary>
            OnSubmit = 3
        }
    }
}
