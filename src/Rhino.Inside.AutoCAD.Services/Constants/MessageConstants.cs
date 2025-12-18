namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A constants class containing common message strings used in the application.
/// </summary>
public class MessageConstants
{
    /// <summary>
    /// Message logged if there is an error loading the material design assemblies.
    /// </summary>
    public const string ErrorLoadingMaterialDesign =
        "The was an error loading materialDeisign Assemblies";

    /// <summary>
    /// The message logged if the logger is initialized more than once.
    /// </summary>
    public const string LoggerServiceAlreadyInitialized =
        "Logger has already been initialized.";

    /// <summary>
    /// The message logged if the logger is used before being initialized.
    /// </summary>
    public const string LoggerServiceNotInitialized =
        "Logger has not been initialized. Call Initialize() first.";

    /// <summary>
    /// The error message when user settings fail to deserialize.
    /// </summary>
    public const string UserSettingDeserializeError = "Unable to deserialize user settings";

    /// <summary>
    /// The error message when the Rhino Inside ribbon tab is not loaded.
    /// </summary>
    public const string RhinoInsideTabNotLoadedError = "Rhino Inside Ribbon Tab not loaded";
}