using Rhino.Inside.AutoCAD.Core.Interfaces;

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
    /// The body of the exception message used in the <see cref="IDebugInfo"/> class.
    /// </summary>
    public const string ExceptionBody =
        "Exception: {0}, \n Stack: {1}, \n\n\n InnerMessage: {2}, \n InnerStack: {3}";

    /// <summary>
    /// The message logged if there is an error initializing the logger.
    /// </summary>
    public const string LoggerInitializationError = "Error with logger Initialization.";

    /// <summary>
    /// The message logged when a user interacts with a UI command.
    /// </summary>
    public const string UserHasInteractedLogMessage =
        "The user has interacted with the UI command: {0} in {1}";

    /// <summary>
    /// The Method name for constructors in Log messages.
    /// </summary>
    public const string ConstructorMethodName = "Constructor";

    /// <summary>
    /// The format for logging exception messages.
    /// </summary>
    public const string ExceptionMessageFormat = "\nException: {0}";

    /// <summary>
    /// The format for logging inner exception messages.
    /// </summary>
    public const string InnerExceptionFormat = "\nInner Exception: {0}";

    /// <summary>
    /// The format for logging stack traces.
    /// </summary>
    public const string StackTraceFormat = "\nStack Trace: {0}";

    /// <summary>
    /// The format for logging piped log messages in a flattened structure.
    /// </summary>
    public const string LogPipMessageFlattenedFormat =
        "SupportApplicationName: {0} | Timestamp: {1} | Level: {2} | Message: {3} <br>";

    /// <summary>
    /// The title of the error dialog when there is an error loading <see
    /// cref="ISupportedApplication"/>s 
    /// </summary>
    public const string SupportApplicationLoadingErrorMessage =
        "Error Loading Supported Applications";

    /// <summary>
    /// The filter used to collect only the <see cref="ISupportedApplication"/>s from the
    /// <see cref="IApplicationDirectories"/> folder.
    /// </summary>
    public const string SupportedAppPatten = "*_SupportedApplication.json";

    /// <summary>
    /// The error message when saving scope data fails.
    /// </summary>
    public const string ErrorSavingScopeMessage = "There was an error saving the Scope.json data";

    /// <summary>
    /// The error message when deserializing scope JSON fails.
    /// </summary>
    public const string JsonDeserializeErrorMessage = "There was an error deserializing the Scope.json to a ScopeDefinition, ensure the json is valid and retry";

    /// <summary>
    /// The title for save error dialogs.
    /// </summary>
    public const string ErrorSavingScopeTitle = "Error Saving";

    /// <summary>
    /// The title for JSON parsing error dialogs.
    /// </summary>
    public const string JsonErrorTitle = "Json Error";

    /// <summary>
    /// The filter used to collect only the <see cref="IUserTestSuite"/>s from the
    /// <see cref="IApplicationDirectories"/> folder.
    /// </summary>
    public const string UserTestFilesPattern = "*_UserTests.json";
}