namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Specifies the modes in which Rhino.Inside can operate within AutoCAD.
/// </summary>
public enum RhinoInsideMode
{
    /// <summary>
    /// Operates in headless mode, without displaying any user interface.
    /// </summary>
    Headless,

    /// <summary>
    /// Operates in windowed mode, displaying the Rhino user interface.
    /// </summary>
    Windowed,

    /// <summary>
    /// Operates in windowed mode with a splash screen displayed during initialization.
    /// </summary>
    WithSplash
}