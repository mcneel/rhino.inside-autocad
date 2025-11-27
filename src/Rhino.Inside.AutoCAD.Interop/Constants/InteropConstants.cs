using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constant class for interop-related values.
/// </summary>
public class InteropConstants
{
    /// <summary>
    /// The internal unit system used by AWI applications.
    /// </summary>
    public const UnitSystem FallbackUnitSystem = UnitSystem.Millimeters;

    /// <summary>
    /// The name of the AWI settings JSON file.
    /// </summary>
    public const string SettingCoreJsonName = "SettingsCore.json";

    /// <summary>
    /// The name of the root installation folder the software is stored.
    /// </summary>
    public const string RootInstallFolderName = "AWI Applications.bundle";

    /// <summary>
    /// The directory of the AutoCAD plugin directory.
    /// </summary>
    public const string AutoCadPluginDirectory = "Autodesk\\ApplicationPlugins\\";

    /// <summary>
    /// The folder storing the application assemblies.
    /// </summary>
    public const string AssemblyFolderName = "Win64";

    /// <summary>
    /// The name of the client folder.
    /// </summary>
    public const string ClientFolderName = "AWI";

    /// <summary>
    /// The name that is prefixed to the version number of the deployment.
    /// </summary>
    public const string PackagePrefixName = "AWI.Applications.";

    /// <summary>
    /// The internal name of the application.
    /// </summary>
    public const string ApplicationName = "BIMORPH_AWI_APPLICATIONS";

    /// <summary>
    /// The name of the folder in the solution storing the deployment.
    /// </summary>
    public const string DeploymentFolderName = "Deployment";

    /// <summary>
    /// The name of the application resources folder.
    /// </summary>
    public const string ResourcesFolderName = "Resources";

    /// <summary>
    /// The prefix and extension name of the log file.
    /// </summary>
    public const string LogFileExtension = "_Log.txt";

    /// <summary>
    /// The factor that is used to calculate the dimensions (height and width)
    /// of the view in <see cref="IEntityHighlighter"/> when zooming to a
    /// specific <see cref="IEntity"/>. It determines the degree of expansion
    /// of the view, ensuring that the entity is sufficiently visible after the
    /// zoom operation.
    /// </summary>
    public const double ZoomExpansionFactor = 3;

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
    /// An arbitrary prime number used to help distribute a hash space.
    /// </summary>
    public const int HashPrimeNumber = 31;

    /// <summary>
    /// An arbitrary prime number used as the initializer for a hash code to
    /// assist with generating unique hashes.
    /// </summary>
    public const int HashPrimeRoot = 17;
}
