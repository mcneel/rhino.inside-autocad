namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a splash screen window launcher.
/// </summary>
public interface ILoadingScreenManager : IDisposable
{
    /// <summary>
    /// Displays the <paramref name="validationLogger"/> error message
    /// to the user in the splash screen UI.
    /// </summary>
    void ShowFailedValidationInfo(IValidationLogger validationLogger);

    /// <summary>
    /// Displays the exception message to the user in the splash screen UI.
    /// </summary>
    void ShowExceptionInfo();

    /// <summary>
    /// Displays the splash screen to the user.
    /// </summary>
    void Show();

    /// <summary>
    /// Closes the splash screen.
    /// </summary>
    void Close();
}