namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A services interface for handling group redirects and
/// resolving assemblies which the app depends on that conflict with AutoCAD.
/// </summary>
public interface IAssemblyManager
{
    /// <summary>
    /// Shuts down this service.
    /// </summary>
    void ShutDown();
}