namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// The outcome of deciding which Rhino installation to bind a session to.
/// </summary>
/// <seealso cref="Interfaces.IRhinoVersionSelection"/>
public enum RhinoVersionResolution
{
    /// <summary>
    /// An installation was chosen and can be bound.
    /// </summary>
    Resolved = 0,

    /// <summary>
    /// No Rhino version this build can host is installed.
    /// </summary>
    NotInstalled = 1,

    /// <summary>
    /// Every installed Rhino version this build could host is older than the minimum
    /// RhinoCommon version, so the user must update Rhino before the plugin can load.
    /// </summary>
    UpdateRequired = 2,

    /// <summary>
    /// The user cancelled the version selection dialog without choosing a version.
    /// </summary>
    Cancelled = 3
}
