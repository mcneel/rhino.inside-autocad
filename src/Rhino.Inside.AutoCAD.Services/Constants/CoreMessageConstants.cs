namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class containing core message constants used in the services layer.
/// </summary>
public class CoreMessageConstants
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
    /// The message logged when a user interacts with a UI command.
    /// </summary>
    public const string UserHasInteractedLogMessage =
        "The user has interacted with the UI command: {0} in {1}";

    /// <summary>
    /// The format for logging piped log messages in a flattened structure.
    /// </summary>
    public const string LogPipMessageFlattenedFormat =
        "SupportApplicationName: {0} | Timestamp: {1} | Level: {2} | Message: {3} <br>";

}