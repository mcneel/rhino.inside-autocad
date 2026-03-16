namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a splash screen window launcher.
/// </summary>
public interface ILoadingScreenManager : IDisposable
{
    /// <summary>
    /// Displays the <paramref name="startUpLogger"/> error message
    /// to the user in the splash screen UI.
    /// </summary>
    void ShowFailedValidationInfo(IStartUpLogger startUpLogger);

    /// <summary>
    /// Displays the exception message to the user in the splash screen UI.
    /// </summary>
    void ShowExceptionInfo();

    /// <summary>
    /// Displays the failure message to the user in the splash screen UI.
    /// </summary>
    void ShowFailureMessage(string message);

    /// <summary>
    /// Displays the splash screen to the user.
    /// </summary>
    void Show();

    /// <summary>
    /// Closes the splash screen.
    /// </summary>
    void Close();
}