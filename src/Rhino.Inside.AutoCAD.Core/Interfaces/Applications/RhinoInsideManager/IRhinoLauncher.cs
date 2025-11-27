namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service to launch the Rhino Inside instance, This can be used
/// to launch headless or windowed Rhino Instances.
/// </summary>
public interface IRhinoLauncher
{
    /// <summary>
    /// Launches the Rhino Inside instance.
    /// </summary>
    void Launch(RhinoInsideMode mode);
}
