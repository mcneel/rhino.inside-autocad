namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service class which manages dialogs and posts dialog windows
/// to the user in the UI such as notifications and settings.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// The event handler which is raised when the <see cref="CurrentDialogView"/>
    /// is hidden.
    /// </summary>
    event EventHandler? DialogHidden;

    /// <summary>
    /// The event handler which is raised when a notification is posted.
    /// </summary>
    event EventHandler? DialogPosted;

    /// <summary>
    /// The current page object.
    /// </summary>
    object? CurrentDialogView { get; set; }

    /// <summary>
    /// The current <see cref="INotificationMessage"/> posted to this
    /// <see cref="IDialogService"/>.
    /// </summary>
    INotificationMessage? Notification { get; }

    /// <summary>
    /// True if a notification is currently posted and visible in the UI otherwise false.
    /// </summary>
    bool DialogShowing { get; }

    /// <summary>
    /// Hides any message posted to this <see cref="IDialogService"/>.
    /// </summary>
    void Hide();

    /// <summary>
    /// Posts a <see cref="INotificationMessage"/> to the <see cref="IDialogService"/>.
    /// </summary>
    void Post(Action? action, string message, string title, string buttonContent,
        bool isWarning = false);

    /// <summary>
    /// Posts a <see cref="INotificationMessage"/> to the <see cref="IDialogService"/>.
    /// </summary>
    void Post(Action? action, Action? cancelAction, string message, string title, string buttonContent,
        bool isWarning = false);

    /// <summary>
    /// Posts a notification to the <see cref="IDialogService"/> with an
    /// OK button to dismiss the notification.
    /// </summary>
    void Post(string message, string title, bool isWarning = false);

    /// <summary>
    /// Posts a failure notification to the <see cref="IDialogService"/> with the
    /// provided <paramref name="message"/>.
    /// </summary>
    void PostFailure(string message, string title = "Failure Message");

    /// <summary>
    /// Posts a warning notification to the <see cref="IDialogService"/> with the
    /// exception <see cref="IDebugInfo"/> which contains the exception
    /// and debug information.
    /// </summary>
    void PostException(IDebugInfo debugInfo);

    /// <summary>
    /// Posts a notification to the <see cref="IDialogService"/> to update the
    /// application with the provided <paramref name="action"/>.
    /// </summary>
    void PostUpdateApplication(Action? action);

    /// <summary>
    /// Posts a custom dialog to the <see cref="IDialogService"/>.
    /// </summary>
    void Post(string notificationKey);

    /// <summary>
    /// Posts a choice dialog to the user this dialog will work with async/await flows.
    /// </summary>
    Task<bool> PostChoiceAsync(string message, string title, string buttonContent, bool isWarning = false);
}