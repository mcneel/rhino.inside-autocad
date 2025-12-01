namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A detailed <see cref="INotificationMessage"/> which includes a postable
/// UI message, message title, and button content name. This Notification gives
/// the user a choice with two buttons: action and cancel action.
/// </summary>
public interface IChoiceNotificationMessage : INotificationMessage
{
    /// <summary>
    /// The task to undertake when the user dismisses this notification. By Canceling.
    /// </summary>
    Action? CancelAction { get; set; }

    /// <summary>
    /// The content (display name) of the cancel button in the notification
    /// dialog. 
    /// </summary>
    string CancelButtonContent { get; set; }
}