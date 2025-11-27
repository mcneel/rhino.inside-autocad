using Bimorph.Core.Services.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;

/// <summary>
/// The application entry point for Bimorph.Core.Services applications.
/// </summary>
public interface IRhinoInsideAutoCadApplication : IBimorphApplication
{
    /// <summary>
    /// The manager for all application and user settings.
    /// </summary>
    IRhinoInsideAutoCadSettingsManager SettingsManager { get; }

    /// <summary>
    /// The file resource manager for obtaining essential file resources.
    /// </summary>
    IRhinoInsideAutoCadFileResourceManager FileResourceManager { get; }

    /// <summary>
    /// The principal controller for Rhino.Inside functionality. This manager
    /// provides access to both Rhino and autocad, and the AutoCAD-Rhino bridge.
    /// </summary>
    IRhinoInsideManager RhinoInsideManager { get; }

    /// <summary>
    /// Terminate the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    void Terminate();
}