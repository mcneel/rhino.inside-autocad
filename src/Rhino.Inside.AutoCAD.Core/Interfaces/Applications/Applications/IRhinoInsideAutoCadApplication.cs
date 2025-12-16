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
    /// The manager for the Support Dialog.
    /// </summary>
    ISupportDialogManager SupportDialogManager { get; }

    /// <summary>
    /// The <see cref="IBrepConverterRunner"/> used to convert Breps between
    /// via the Autocad Import command. Autocad has no .Net or ObjectArx exposed
    /// method to convert Breps into Autocad Solids, so this runner uses the
    /// Autocad Import command to perform the conversion.
    /// </summary>
    IBrepConverterRunner BrepConverterRunner { get; }

    /// <summary>
    /// Show an alert dialog with the given message.
    /// </summary>
    void ShowAlertDialog(string message);

    /// <summary>
    /// Terminate the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    void Terminate();
}