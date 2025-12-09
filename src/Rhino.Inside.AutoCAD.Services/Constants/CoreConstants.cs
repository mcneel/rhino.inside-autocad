namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Application constants.
/// </summary>
public class CoreConstants
{
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

}