using System.ComponentModel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which provides a wrapper around a WPF Window,
/// enabling loosely coupled interactions within the application.
/// </summary>
public interface IWindow
{
    /// <summary>
    /// The closing window event handler.
    /// </summary>
    event CancelEventHandler Closing;

    /// <summary>
    /// The max height of the window.
    /// </summary>
    double MaxHeight { get; set; }

    /// <summary>
    /// The close window method.
    /// </summary>
    void Close();

    /// <summary>
    /// Displays a non-modal window to the user.
    /// </summary>
    void Show();
}