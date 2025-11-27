namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Specifies how a window should be displayed or hidden.
/// Used with the Win32 ShowWindow function to control window visibility and state.
/// </summary>
public enum WindowShowStyle
{
    /// <summary>
    /// Hides the window and activates another window.
    /// The window becomes completely invisible and is removed from the taskbar.
    /// </summary>
    Hide = 0,

    /// <summary>
    /// Activates the window and displays it in its current size and position.
    /// If the window is minimized or maximized, the system restores it to its original size and position.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Activates the window and displays it as a minimized window.
    /// The window is reduced to a taskbar button.
    /// </summary>
    Minimized = 2,

    /// <summary>
    /// Activates the window and displays it as a maximized window.
    /// The window fills the entire screen.
    /// </summary>
    Maximized = 3,

    /// <summary>
    /// Displays the window in its most recent size and position without activating it.
    /// Similar to Normal, but does not make the window the foreground window.
    /// </summary>
    ShowNoActivate = 4,

    /// <summary>
    /// Activates the window and displays it in its current size and position.
    /// </summary>
    Show = 5,

    /// <summary>
    /// Minimizes the specified window and activates the next top-level window in the Z order.
    /// </summary>
    Minimize = 6,

    /// <summary>
    /// Displays the window as a minimized window without activating it.
    /// The window remains in its current position in the Z order.
    /// </summary>
    ShowMinimizedNoActivate = 7,

    /// <summary>
    /// Displays the window in its current size and position without activating it.
    /// </summary>
    ShowNA = 8,

    /// <summary>
    /// Activates and displays the window. If the window is minimized or maximized,
    /// the system restores it to its original size and position.
    /// </summary>
    Restore = 9,

    /// <summary>
    /// Sets the show state based on the state specified in the STARTUPINFO structure
    /// passed to the CreateProcess function by the program that started the application.
    /// </summary>
    ShowDefault = 10,

    /// <summary>
    /// Minimizes a window, even if the thread that owns the window is not responding.
    /// Should only be used when minimizing windows from a different thread.
    /// </summary>
    ForceMinimize = 11
}