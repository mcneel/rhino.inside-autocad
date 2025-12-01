using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Runtime.InteropServices;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoWindowManager"/>
public class RhinoWindowManager : IRhinoWindowManager
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr windowHandle, int windowShowStyle);

    /// <summary>
    /// The pointer to the Rhino main window.
    /// </summary>
    private IntPtr _mainWindow;

    /// <summary>
    /// Constructs a new <see cref="IRhinoWindowManager"/> instance. This is the
    /// default state and does not have a window associated with it.
    /// </summary>
    public RhinoWindowManager()
    {
        _mainWindow = IntPtr.Zero;
    }

    /// <inheritdoc />
    public void SetWindow(IntPtr mainWindow)
    {
        _mainWindow = mainWindow;
    }

    /// <inheritdoc />
    public void HideWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Hide);
    }

    /// <inheritdoc />
    public void ShowWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Show);
    }
}
