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
    /// The error message when the Rhino Inside ribbon tab is not loaded.
    /// </summary>
    public const string RhinoInsideTabNotLoadedError = "Rhino Inside Ribbon Tab not loaded";

    /// <summary>
    /// The Error message format for WCF preload failures. The placeholder {0} is replaced
    /// with the specific error details.
    /// </summary>
    public const string WcfErrorMessage = "WCF preload failed: {0}";

    /// <summary>
    /// The error message format when a legacy plugin's resources cannot be found. The
    /// placeholder {0} is replaced with the base name and {1} with the assembly name.
    /// </summary>
    public const string LegacyResourcesNotFound =
        "Legacy resources '{0}' were not found in assembly '{1}'; its icons will be blank.";

    /// <summary>
    /// The error message format when a legacy plugin's resources class cannot be given a
    /// <c>LegacyResourceManager</c>. The placeholder {0} is replaced with the class or
    /// assembly name.
    /// </summary>
    public const string LegacyResourcesAttachFailed =
        "Could not read the legacy resources of '{0}' without BinaryFormatter; its icons may be blank.";
}