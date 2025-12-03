namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The application entry point for Rhino.Inside.AutoCAD applications.
/// </summary>
public interface IRhinoInsideAutoCadApplication
{
    /// <summary>
    /// The bootstrapper for the host application.
    /// </summary>
    IBootstrapper Bootstrapper { get; }

    /// <summary>
    /// The core application services.
    /// </summary>
    IApplicationServicesCore ApplicationServicesCore { get; }

    /// <summary>
    /// The application configuration settings.
    /// </summary>
    IApplicationConfig ApplicationConfig { get; }

    /// <summary>
    /// The manager for all application and user settings.
    /// </summary>
    ISettingsManager SettingsManager { get; }

    /// <summary>
    /// The file resource manager for obtaining essential file resources.
    /// </summary>
    IFileResourceManager FileResourceManager { get; }

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