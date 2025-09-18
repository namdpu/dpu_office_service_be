using System.Text.Json.Serialization;

namespace OfficeService.DAL.DTOs.Requests
{
    public class RequestToken
    {
        public int? ExpiresIn { get; set; }
    }

    /// <summary>
    /// The main class that contains all the configuration parameters for the document editor.
    /// </summary>
    public class Config
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
        public Document Document { get; set; }

        /// <summary>
        /// Configuration for the editor.
        /// </summary>
        public EditorConfig? EditorConfig { get; set; }
    }

    /// <summary>
    /// Defines the parameters pertaining to the document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Defines the type of the file for the source viewed or edited document. Must be lowercase.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Defines the unique document identifier used by the service to recognize the document.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Defines the desired file name for the viewed or edited document which will also be used as file name when the document is downloaded. The length is limited to 128 symbols.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Defines the absolute URL where the source viewed or edited document is stored. Be sure to add a token when using local links.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Defines the version of file
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Information about the latest version of the document.
        /// </summary>
        public Info? Info { get; set; }

        /// <summary>
        /// The permissions for the document.
        /// </summary>
        public Permissions? Permissions { get; set; }
    }

    /// <summary>
    /// Defines additional information about the document.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Defines the name of the document owner/creator.
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// Defines the name of the document owner/creator.
        /// </summary>
        public string? Owner { get; set; }

        /// <summary>
        /// Defines the document uploading date.
        /// </summary>
        public string? Uploaded { get; set; }

        /// <summary>
        /// Marks the document as a favorite.
        /// </summary>
        public bool? Favorite { get; set; }

        /// <summary>
        /// The folder where the document is stored.
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// Displays the information about the settings which allow to share the document with other users.
        /// </summary>
        public SharingSettings? SharingSettings { get; set; }
    }

    /// <summary>
    /// Defines the user's permissions for the document.
    /// </summary>
    public class Permissions
    {
        /// <summary>
        /// Defines if the chat functionality is enabled in the document or not. In case the chat permission is set to true, the Chat menu button will be displayed. The default value is true.
        /// </summary>
        public bool? Chat { get; set; }

        /// <summary>
        /// Defines if the document can be commented or not. In case the commenting permission is set to "true" the document side bar will contain the Comment menu option; the document commenting will only be available for the document editor if the mode parameter is set to edit. The default value coincides with the value of the edit parameter.
        /// </summary>
        public bool? Comment { get; set; }

        /// <summary>
        /// Defines the groups whose comments the user can edit, remove and/or view.
        /// </summary>
        public CommentGroups? CommentGroups { get; set; }

        /// <summary>
        /// Defines if the content can be copied to the clipboard or not. In case the parameter is set to false, pasting the content will be available within the current document editor only.
        /// </summary>
        public bool? Copy { get; set; }

        /// <summary>
        /// Defines if the user can delete only his/her comments.
        /// </summary>
        public bool? DeleteCommentAuthorOnly { get; set; }

        /// <summary>
        /// Defines if the document can be downloaded or only viewed or edited online.
        /// </summary>
        public bool? Download { get; set; }

        /// <summary>
        /// Defines if the document can be edited or only viewed.
        /// </summary>
        public bool? Edit { get; set; }

        /// <summary>
        /// Defines if the user can edit only his/her comments.
        /// </summary>
        public bool? EditCommentAuthorOnly { get; set; }

        /// <summary>
        /// Defines if the forms can be filled.
        /// </summary>
        public bool? FillForms { get; set; }

        /// <summary>
        /// Defines if the content control settings can be changed. Content control modification will only be available for the document editor if the mode parameter is set to edit.
        /// </summary>
        public bool? ModifyContentControl { get; set; }

        /// <summary>
        /// Defines if the filter can applied globally (true) affecting all the other users, or locally (false), i.e. for the current user only. Filter modification will only be available for the spreadsheet editor if the mode parameter is set to edit.
        /// </summary>
        public bool? ModifyFilter { get; set; }

        /// <summary>
        /// Defines if the document can be printed or not.
        /// </summary>
        public bool? Print { get; set; }

        /// <summary>
        /// Defines if the Protection tab on the toolbar and the Protect button in the left menu are displayed (true) or hidden (false).
        /// </summary>
        public bool? Protect { get; set; }

        /// <summary>
        /// Defines if the document can be reviewed or not.
        /// </summary>
        public bool? Review { get; set; }
        /// <summary>
        /// Defines the groups whose changes the user can accept/reject. The [""] value means that the user can review changes made by someone who belongs to none of these groups (for example, if the document is reviewed in third-party editors). If the value is [], the user cannot review changes made by any group. If the value is "" or not specified, then the user can review changes made by any user.
        /// </summary>
        public string[]? ReviewGroups { get; set; }

        /// <summary>
        /// Defines the groups of users whose information is displayed in the editors.
        /// </summary>
        public string[]? UserInfoGroups { get; set; }
    }

    /// <summary>
    /// Defines parameters for customizing the editor's interface and functionality.
    /// </summary>
    public class EditorConfig
    {
        /// <summary>
        /// Specifies the data received from the document editing service using the onMakeActionLink event or the onRequestSendNotify event in data.actionLink parameter, which contains the information about the action in the document that will be scrolled to.
        /// </summary>
        public string? ActionLink { get; set; }

        /// <summary>
        /// Specifies absolute URL to the document storage service (which must be implemented by the software integrators who use ONLYOFFICE Docs on their own server).
        /// </summary>
        public string? CallbackUrl { get; set; }

        /// <summary>
        /// Defines the co-editing mode (Fast or Strict) and the possibility to change it.
        /// </summary>
        public CoEditing? CoEditing { get; set; }

        /// <summary>
        /// Defines the absolute URL of the document where it will be created and available after creation.
        /// </summary>
        public string? CreateUrl { get; set; }

        /// <summary>
        /// Defines the editor interface language (if some other languages other than English are present). Is set using the two letter (de, ru, it, etc.) language codes. The default value is "en".
        /// </summary>
        public string? Lang { get; set; }

        /// <summary>
        /// Defines the editor opening mode. Can be either view to open the document for viewing, or edit to open the document in the editing mode allowing to apply changes to the document data.
        /// </summary>
        public string? Mode { get; set; }

        /// <summary>
        /// Defines the presence or absence of the documents in the Open Recent... menu option.
        /// </summary>
        public Recent[]? Recent { get; set; }

        /// <summary>
        /// Defines the default display format for currency and date and time (in the Spreadsheet Editor only). Is set using the four letter (en-US, fr-FR, etc.) language codes. For the default value the lang parameter is taken, or, if no regional setting corresponding to the lang value is available, en-US is used.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Defines the presence or absence of the templates in the Create New... menu option.
        /// </summary>
        public Template? Templates { get; set; }

        /// <summary>
        /// Defines the user currently viewing or editing the document.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Configuration for editor customization.
        /// </summary>
        public Customization? Customization { get; set; }
    }

    /// <summary>
    /// Defines the user currently viewing or editing the document.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The full name of the user. The length is limited to 128 symbols.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The group (or several groups separated with commas) the user belongs to.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// The user's ID.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The path to the user's avatar.
        /// </summary>
        public string? Image { get; set; }
    }

    /// <summary>
    /// Represents the standard branding customization options for ONLYOFFICE Docs editor.
    /// </summary>
    public class Customization
    {
        /// <summary>
        /// Configuration for requesting an anonymous user name (e.g., "Guest").
        /// </summary>
        public AnonymousConfig? Anonymous { get; set; }

        /// <summary>
        /// Enables or disables the Autosave menu option. Required for Fast co-editing.
        /// </summary>
        public bool? Autosave { get; set; }

        /// <summary>
        /// Configuration for the close button in the editor header.
        /// </summary>
        public CloseConfig? Close { get; set; }

        /// <summary>
        /// Shows or hides the Comments button in the toolbar.
        /// </summary>
        public bool? Comments { get; set; }

        /// <summary>
        /// Enables compact header layout by moving action buttons next to the logo.
        /// </summary>
        public bool? CompactHeader { get; set; }

        /// <summary>
        /// Enables compact toolbar layout. Also applies to viewer in v8.3+.
        /// </summary>
        public bool? CompactToolbar { get; set; }

        /// <summary>
        /// Restricts features to OOXML-compatible ones only.
        /// </summary>
        public bool? CompatibleFeatures { get; set; }

        /// <summary>
        /// Company or individual branding displayed in the About dialog.
        /// </summary>
        public CustomerConfig? Customer { get; set; }

        /// <summary>
        /// Enables or disables tips, role manager, spellcheck, tab style changes.
        /// </summary>
        public FeaturesConfig? Features { get; set; }

        /// <summary>
        /// Shows/hides the Feedback button or customizes the feedback URL.
        /// </summary>
        public FeedbackConfig? Feedback { get; set; }

        /// <summary>
        /// Enables force-save events to the callback handler.
        /// </summary>
        public bool? Forcesave { get; set; }

        /// <summary>
        /// Uses Western font sizes in Simplified Chinese UI.
        /// </summary>
        public bool? ForceWesternFontSize { get; set; }

        /// <summary>
        /// Configuration for the Go back / Open file location button.
        /// </summary>
        public GoBackConfig? GoBack { get; set; }

        /// <summary>
        /// Shows or hides the Help button in the toolbar.
        /// </summary>
        public bool? Help { get; set; }

        /// <summary>
        /// Hides the notes panel in the presentation editor when opening a file.
        /// </summary>
        public bool? HideNotes { get; set; }

        /// <summary>
        /// Hides the right menu on first load.
        /// </summary>
        public bool? HideRightMenu { get; set; }

        /// <summary>
        /// Hides the ruler in document or presentation editors.
        /// </summary>
        public bool? HideRulers { get; set; }

        /// <summary>
        /// Embedding mode for the editor ("embed" prevents scroll focus capture).
        /// </summary>
        public string? IntegrationMode { get; set; }

        /// <summary>
        /// Custom logo images and settings.
        /// </summary>
        public LogoConfig? Logo { get; set; }

        /// <summary>
        /// Enables or disables macros execution when opening the editor.
        /// </summary>
        public bool? Macros { get; set; }

        /// <summary>
        /// Defines macros execution mode: disable, warn, enable.
        /// </summary>
        public string? MacrosMode { get; set; }

        /// <summary>
        /// Enables mention notifications with or without access rights.
        /// </summary>
        public bool? MentionShare { get; set; }

        /// <summary>
        /// Mobile-specific customization options.
        /// </summary>
        public MobileConfig? Mobile { get; set; }

        /// <summary>
        /// Defines if plugins will be launched and available.
        /// </summary>
        public bool? Plugins { get; set; }

        /// <summary>
        /// Defines if plugins will be launched and available.
        /// </summary>
        public string PointerMode { get; set; }

        /// <summary>
        /// Review mode customization options.
        /// </summary>
        public ReviewConfig? Review { get; set; }

        /// <summary>
        /// Shows or hides horizontal scroll in the spreadsheet editor.
        /// </summary>
        public bool? ShowHorizontalScroll { get; set; }

        /// <summary>
        /// Shows or hides vertical scroll in the spreadsheet editor.
        /// </summary>
        public bool? ShowVerticalScroll { get; set; }

        /// <summary>
        /// Background color for slide shows in the presentation editor.
        /// </summary>
        public string? SlidePlayerBackground { get; set; }

        /// <summary>
        /// Defines the Complete & Submit button settings.
        /// </summary>
        public SubmitForm? SubmitForm { get; set; }

        /// <summary>
        /// Defines if the document title is visible on the top toolbar (false) or hidden (true).
        /// </summary>
        public bool? ToolbarHideFileName { get; set; }

        /// <summary>
        /// Sets the editor UI theme by ID.
        /// </summary>
        public string? UiTheme { get; set; }

        /// <summary>
        /// Measurement units for rulers and dialogs: "cm", "pt", "inch".
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Heading color in the document editor.
        /// </summary>
        public string? WordHeadingsColor { get; set; }

        /// <summary>
        /// Document zoom percentage; -1 (fit to page), -2 (fit width).
        /// </summary>
        public int? Zoom { get; set; }
    }

    /// <summary>
    /// Configuration for anonymous user display.
    /// </summary>
    public class AnonymousConfig
    {
        /// <summary>
        /// If true, requests anonymous username input.
        /// </summary>
        public bool? Request { get; set; } = true;

        /// <summary>
        /// Postfix label for anonymous users.
        /// </summary>
        public string? Label { get; set; } = "Guest";
    }

    /// <summary>
    /// Settings for the close button.
    /// </summary>
    public class CloseConfig
    {
        /// <summary>
        /// Whether the close button is visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Text/tooltip for the close button.
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Company or individual branding information.
    /// </summary>
    public class CustomerConfig
    {
        public string? Address { get; set; }
        public string? Info { get; set; }
        public string? Logo { get; set; }
        public string? LogoDark { get; set; }
        public string? Mail { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Www { get; set; }
    }

    /// <summary>
    /// Additional features configuration.
    /// </summary>
    public class FeaturesConfig
    {
        /// <summary>
        /// Enables or disables tips.
        /// </summary>
        public bool? FeaturesTips { get; set; }

        /// <summary>
        /// Enables or disables roles manager.
        /// </summary>
        public bool? Roles { get; set; }

        /// <summary>
        /// Spellchecker settings: bool or object with mode.
        /// </summary>
        public object? Spellcheck { get; set; }

        /// <summary>
        /// Background color configuration for tabs.
        /// </summary>
        public TabBackgroundConfig? TabBackground { get; set; }

        /// <summary>
        /// Style configuration for tabs.
        /// </summary>
        public TabStyleConfig? TabStyle { get; set; }

        /// <summary>
        /// Tab background configuration.
        /// </summary>
        public class TabBackgroundConfig
        {
            public string? Mode { get; set; }
            public bool? Change { get; set; }
        }

        /// <summary>
        /// Tab style configuration.
        /// </summary>
        public class TabStyleConfig
        {
            public string? Mode { get; set; }
            public bool? Change { get; set; }
        }
    }

    /// <summary>
    /// Feedback button configuration.
    /// </summary>
    public class FeedbackConfig
    {
        public string? Url { get; set; }
        public bool? Visible { get; set; }
    }

    /// <summary>
    /// "Go back" button configuration.
    /// </summary>
    public class GoBackConfig
    {
        public bool? Blank { get; set; }
        public string? Text { get; set; }
        public string? Url { get; set; }
    }

    /// <summary>
    /// Custom logo configuration.
    /// </summary>
    public class LogoConfig
    {
        public string? Image { get; set; }
        public string? ImageDark { get; set; }
        public string? Url { get; set; }
        public bool? Visible { get; set; }
    }

    /// <summary>
    /// Mobile-specific settings.
    /// </summary>
    public class MobileConfig
    {
        public bool? ForceView { get; set; }
        public bool? Info { get; set; }
        public bool? StandardView { get; set; }
    }

    /// <summary>
    /// Review mode configuration.
    /// </summary>
    public class ReviewConfig
    {
        public bool? HideReviewDisplay { get; set; }
        public bool? HoverMode { get; set; }
        public string? ReviewDisplay { get; set; }
        public bool? ShowReviewChanges { get; set; }
        public bool? TrackChanges { get; set; }
    }

    /// <summary>
    /// Defines the Complete & Submit button settings.
    /// </summary>
    public class SubmitForm
    {
        public bool? Visible { get; set; }
        public string? ResultMessage { get; set; }
    }

    /// <summary>
    /// Displays the information about the settings which allow to share the document with other users.
    /// </summary>
    public class SharingSettings
    {
        /// <summary>
        /// Changes the user icon to the link icon.
        /// </summary>
        [JsonPropertyName("isLink")]
        public bool IsLink { get; set; }

        /// <summary>
        /// Changes the user icon to the link icon.
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions { get; set; }

        /// <summary>
        /// Changes the user icon to the link icon.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; }
    }

    /// <summary>
    /// Displays the information about the settings which allow to share the document with other users.
    /// </summary>
    public class CommentGroups
    {
        /// <summary>
        /// The user can edit comments made by other users.
        /// </summary>
        public string[] Edit { get; set; }

        /// <summary>
        /// The user can remove comments made by other users.
        /// </summary>
        public string[] Remove { get; set; }

        /// <summary>
        /// Changes the user icon to the link icon.
        /// </summary>
        public string User { get; set; }
    }

    /// <summary>
    /// Defines the co-editing mode (Fast or Strict) and the possibility to change it.
    /// </summary>
    public class CoEditing
    {
        /// <summary>
        /// The co-editing mode (fast or strict). The default value is fast.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Defines if the co-editing mode can be changed in the editor interface or not.
        /// </summary>
        public bool Change { get; set; }
    }

    /// <summary>
    /// Defines the presence or absence of the documents in the Open Recent... menu option.
    /// </summary>
    public class Recent
    {
        /// <summary>
        /// The folder where the document is stored (can be empty in case the document is stored in the root folder).
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// The document title that will be displayed in the Open Recent... menu option.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The absolute URL to the document where it is stored.
        /// </summary>
        public string? Url { get; set; }
    }
    /// <summary>
    /// Defines the presence or absence of the documents in the Open Recent... menu option.
    /// </summary>
    /// 
    public class Template
    {
        /// <summary>
        /// The absolute URL to the image for template.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// The template title that will be displayed in the Create New... menu option.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The absolute URL to the document where it will be created and available after creation.
        /// </summary>
        public string? Url { get; set; }
    }
}
