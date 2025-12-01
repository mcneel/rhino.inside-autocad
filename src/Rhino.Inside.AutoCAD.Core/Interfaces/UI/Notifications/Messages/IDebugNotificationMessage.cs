namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A detailed <see cref="IChoiceNotificationMessage"/> used to post notifications about
/// exceptions for debugging.
/// </summary>
public interface IDebugNotificationMessage : IChoiceNotificationMessage
{
    /// <summary>
    /// A debug description of the notification message, typically used for
    /// logging or debugging purposes.
    /// </summary>
    string DebugDescription { get; set; }
}