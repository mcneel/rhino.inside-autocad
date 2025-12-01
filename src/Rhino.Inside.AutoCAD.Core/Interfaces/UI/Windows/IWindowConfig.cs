using System.Windows.Interop;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface providing configuration settings for WPF windows.
/// </summary>
public interface IWindowConfig
{
    /// <summary>
    /// Applies the required configuration to the <see cref="IWindow"/>.
    /// </summary>
    void Apply(IWindow window, RenderMode renderMode = RenderMode.Default);
}