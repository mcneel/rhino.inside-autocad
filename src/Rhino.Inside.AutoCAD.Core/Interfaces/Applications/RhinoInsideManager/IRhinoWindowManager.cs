namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Interface to manage the Rhino main window.
/// </summary>
public interface IRhinoWindowManager
{
    /// <summary>
    /// Sets the Rhino main window.
    /// </summary>
    void SetWindow(IntPtr mainWindow);

    /// <summary>
    /// Shows the Rhino main window if there is one set, if not this method does nothing.
    /// </summary>
    void ShowWindow();

    /// <summary>
    /// Hides the Rhino main window if there is one set, if not this method does nothing.
    /// </summary>
    void HideWindow();
}