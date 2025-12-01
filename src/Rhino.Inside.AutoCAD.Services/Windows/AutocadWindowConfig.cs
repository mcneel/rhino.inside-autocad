using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class providing configuration settings for WPF windows to run
/// within the AutoCAD application.
/// </summary>
public class AutocadWindowConfig : IWindowConfig
{
    private readonly IntPtr _parentWindowHandle;

    /// <summary>
    /// Constructs a new <see cref="AutocadWindowConfig"/>.
    /// </summary>
    public AutocadWindowConfig(IntPtr parentWindowHandle)
    {
        _parentWindowHandle = parentWindowHandle;
    }

    /// <summary>
    /// Applies the required configuration to the <see cref="IWindow"/> to run in
    /// AutoCAD.
    /// </summary>
    /// <remarks>
    /// 1. Sets AutoCAD as the owner of the <paramref name="window"/> so it always stays
    /// on top of the AutoCAD application.
    /// 2. Sets the <see cref="RenderOptions.ProcessRenderMode"/> to default so hardware
    /// acceleration is used.
    /// 3. Sets the maximum height of the window to sit above the Windows taskbar when
    /// maximized.
    /// </remarks>
    public void Apply(IWindow window, RenderMode renderMode = RenderMode.Default)
    {
        RenderOptions.ProcessRenderMode = renderMode;

        _ = new WindowInteropHelper((Window)window)
        {
            Owner = _parentWindowHandle
        };

        window.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
    }
}