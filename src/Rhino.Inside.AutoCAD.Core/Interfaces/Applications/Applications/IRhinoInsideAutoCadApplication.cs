using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The application entry point for Rhino.Inside.AutoCAD applications.
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
    /// Show an alert dialog with the given message.
    /// </summary>
    void ShowAlertDialog(string message);

    /// <summary>
    /// Terminate the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    void Terminate();
}