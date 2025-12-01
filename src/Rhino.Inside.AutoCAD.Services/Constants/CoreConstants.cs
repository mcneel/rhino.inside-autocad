using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Application constants.
/// </summary>
public class CoreConstants
{
    /// <summary>
    /// The name of the named pipe used for communication between the application
    /// and the Bimorph Reporter application.
    /// </summary>
    public const string BimorphReporterLoggingPipeName = "BimorphLoggingNamedPipe2";

    /// <summary>
    /// The name of the Rhino.Inside.AutoCAD.Core settings JSON file.
    /// </summary>
    public const string SettingCoreJsonName = "SettingsCore.json";

    /// <summary>
    /// The name of the Application settings JSON file. These are the
    /// settings specific to each application.
    /// </summary>
    public const string ApplicationSettingCoreJsonName = "ApplicationsSettingsCore.json";

    /// <summary>
    /// The name of the user settings JSON file.
    /// </summary>
    public const string UserSettingsJsonName = "UserSettings.json";

    /// <summary>
    /// The folder storing the application assemblies.
    /// </summary>
    public const string AssemblyFolderName = "Win64";

    /// <summary>
    /// The name of the application resources folder.
    /// </summary>
    public const string ResourcesFolderName = "Resources";

    /// <summary>
    /// The name of the material design assembly.
    /// </summary>
    public static List<string> MaterialDesignAssemblyNames =
    [
        "MaterialDesignThemes.Wpf.dll",
        "MaterialDesignColors.dll",
        "Microsoft.Xaml.Behaviors.dll"
    ];

    /// <summary>
    /// The name of the Serilog configuration file.
    /// </summary>
    public const string LogConfigName = "SerilogConfig.json";

    /// <summary>
    /// For formatting golden thread log messages.
    /// </summary>
    public const string GoldenThreadLogFormat = "ScopeId: {0} : Class {1} : Method {2} . {3}";

    /// <summary>
    /// The format for logging scope IDs.
    /// </summary>
    public const string ScopeIdFormat = " ScopeId: {0}";

    /// <summary>
    /// The file name for usage metrics.
    /// </summary>
    public const string UsageMetricsFileName = "{0}_UsageMetrics.json";

    /// <summary>
    /// The file name for user tests.
    /// </summary>
    public const string UserTestsFileName = "{0}_UserTests.json";

    /// <summary>
    /// The file name for supported Applications.
    /// </summary>
    public const string SupportedApplicationFileName = "{0}_SupportedApplication.json";

    /// <summary>
    /// The name of the named pipe used for communication between the application
    /// and the Bimorph Reporter application. Local Computer is ".".
    /// </summary>
    public const string BimorphReporterServerName = ".";

    /// <summary>
    /// The name of the Bimorph application folder. This is used for storing application
    /// data which is used as part of Bimorph wider ecosystem, for example the Bimorph
    /// Reporter will look for this folder for thing such as <see
    /// cref="ISupportedApplication"/>, <see cref="IUserTest"/> and other metrics.
    /// </summary>
    public const string BimorphAppDataFolderName = "Bimorph";

    /// <summary>
    /// The name of the <see cref="IUserTest"/> folder. This should be the same across
    /// all applications which use the Bimorph Reporter ecosystem. It is located inside
    /// the Bimorph application data folder.
    /// </summary>
    public const string UserTestFolderName = "Tests";

    /// <summary>
    /// The name of the <see cref="ISupportedApplication"/> folder. This should be the
    /// same across all applications which use the Bimorph Reporter ecosystem. It is
    /// located inside the Bimorph application data folder.
    /// </summary>
    public const string SupportedApplicationFolderName = "SupportedApplications";

    /// <summary>
    /// The name of the <see cref="IUsageRegister"/> folder. This should be the same across
    /// all applications which use the Bimorph Reporter ecosystem. It is located inside
    /// the Bimorph application data folder.
    /// </summary>
    public const string UsageMetricsFolderName = "Usage";

    /// <summary>
    /// The name of the JSON file used to store scope data.
    /// </summary>
    public const string ScopeFileName = "Scope.json";

}