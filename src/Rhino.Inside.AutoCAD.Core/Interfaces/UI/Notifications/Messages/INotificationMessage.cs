namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A detailed <see cref="INotificationMessage"/> which includes a postable
/// UI message, message title, and button content name.
/// </summary>
public interface INotificationMessage
{
    /// <summary>
    /// The task to undertake when the user dismisses this notification. 
    /// </summary>
    Action? Action { get; set; }

    /// <summary>
    /// The message of the notification.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// The title of the notification.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// The content (display name) of the button in the notification
    /// dialog. 
    /// </summary>
    string ButtonContent { get; set; }

    /// <summary>
    /// True if this message contains a warning otherwise false.
    /// </summary>
    bool IsWarning { get; set; }
}