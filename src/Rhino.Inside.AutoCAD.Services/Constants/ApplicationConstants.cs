using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Contains application-wide constant values for the Rhino.Inside.AutoCAD plugin.
/// </summary>
/// <remarks>
/// This class centralizes all string literals, paths, icon URIs, registry keys, and
/// message templates used throughout the application. Constants are grouped by functionality:
/// assembly paths, framework detection, UI elements, Rhino integration, and error messages.
/// </remarks>
public class ApplicationConstants
{
    /// <summary>
    /// The folder name containing platform-specific application assemblies.
    /// </summary>
    /// <remarks>
    /// Value: "Win64". Located under the installation bundle directory.
    /// </remarks>
    public const string AssemblyFolderName = "Win64";

    /// <summary>
    /// Folder name for .NET Framework 4.8 assemblies.
    /// </summary>
    /// <remarks>
    /// Value: "NET48". Used when running under AutoCAD versions targeting .NET Framework.
    /// </remarks>
    /// <seealso cref="Net8FolderName"/>
    /// <seealso cref="Net10FolderName"/>
    public const string Net48FolderName = "NET48";

    /// <summary>
    /// Folder name for .NET 8 assemblies.
    /// </summary>
    /// <remarks>
    /// Value: "NET8". Used when running under AutoCAD versions targeting .NET 8, and under
    /// the 2025/2026 releases Autodesk later moved to .NET 10 without changing their series.
    /// </remarks>
    /// <seealso cref="Net48FolderName"/>
    /// <seealso cref="Net10FolderName"/>
    public const string Net8FolderName = "NET8";

    /// <summary>
    /// Folder name for .NET 10 assemblies.
    /// </summary>
    /// <remarks>
    /// Value: "NET10". Used when running under AutoCAD 2027 (R26.0) and later.
    /// </remarks>
    /// <seealso cref="Net48FolderName"/>
    /// <seealso cref="Net8FolderName"/>
    public const string Net10FolderName = "NET10";

    /// <summary>
    /// Folder name for embedded application resources.
    /// </summary>
    public const string ResourcesFolderName = "Resources";

    /// <summary>
    /// Filename of the core settings JSON configuration file.
    /// </summary>
    /// <remarks>
    /// Value: "SettingsCore.json". Located in the <see cref="ResourcesFolderName"/> folder.
    /// </remarks>
    public const string SettingJsonName = "SettingsCore.json";

    /// <summary>
    /// Filename of the user settings JSON file.
    /// </summary>
    /// <remarks>
    /// Value: "UserSettings.json". Located in the <see cref="ApplicationFolderName"/> folder
    /// under AppData, not in the installation directory, so it survives an upgrade.
    /// </remarks>
    /// <seealso cref="ApplicationFolderName"/>
    public const string UserSettingsJsonName = "UserSettings.json";

    /// <summary>
    /// The lowest AutoCAD Color Index which names a color.
    /// </summary>
    /// <remarks>
    /// 0 is ByBlock and 256 is ByLayer, neither of which means anything to a transient
    /// preview, so the configurable range stops short of both.
    /// </remarks>
    /// <seealso cref="MaxAciColorIndex"/>
    public const int MinAciColorIndex = 1;

    /// <summary>
    /// The highest AutoCAD Color Index which names a color.
    /// </summary>
    /// <seealso cref="MinAciColorIndex"/>
    public const int MaxAciColorIndex = 255;

    /// <summary>
    /// The AutoCAD Color Index the previews of Rhino geometry are drawn in until the user
    /// chooses otherwise.
    /// </summary>
    /// <remarks>
    /// Value: 4 (cyan).
    /// </remarks>
    /// <seealso cref="IUserSettings.RhinoPreviewColorIndex"/>
    public const int DefaultRhinoPreviewColorIndex = 4;

    /// <summary>
    /// The AutoCAD Color Index the previews of Grasshopper geometry are drawn in until the
    /// user chooses otherwise.
    /// </summary>
    /// <remarks>
    /// Value: 1 (red).
    /// </remarks>
    /// <seealso cref="IUserSettings.GrasshopperPreviewColorIndex"/>
    public const int DefaultGrasshopperPreviewColorIndex = 1;

    /// <summary>
    /// The AutoCAD Color Index selected previews are drawn in until the user chooses
    /// otherwise.
    /// </summary>
    /// <remarks>
    /// Value: 2 (yellow).
    /// </remarks>
    /// <seealso cref="IUserSettings.SelectedPreviewColorIndex"/>
    public const int DefaultSelectedPreviewColorIndex = 2;

    /// <summary>
    /// Assembly filenames for Material Design WPF dependencies.
    /// </summary>
    /// <remarks>
    /// These assemblies must be loaded for the WPF UI components to render correctly.
    /// Includes MaterialDesignThemes.Wpf.dll, MaterialDesignColors.dll, and Microsoft.Xaml.Behaviors.dll.
    /// </remarks>
    public static List<string> MaterialDesignAssemblyNames =
    [
        "MaterialDesignThemes.Wpf.dll",
        "MaterialDesignColors.dll",
        "Microsoft.Xaml.Behaviors.dll"
    ];

    /// <summary>
    /// Filename of the Serilog logging configuration file.
    /// </summary>
    /// <remarks>
    /// Value: "SerilogConfig.json". Located in the <see cref="ResourcesFolderName"/> folder.
    /// </remarks>
    public const string LogConfigName = "SerilogConfig.json";

    /// <summary>
    /// Product name as registered in the assembly metadata.
    /// </summary>
    /// <remarks>
    /// Value: "Rhino.Inside.AutoCAD". Must match the Product name in Directory.Build.props.
    /// </remarks>
    public const string ProductName = "Rhino.Inside.AutoCAD";

    /// <summary>
    /// Application identifier used for internal references and folder names.
    /// </summary>
    /// <remarks>
    /// Value: "RhinoInsideAutoCAD". A compact form without dots for filesystem compatibility.
    /// </remarks>
    public const string ApplicationName = "RhinoInsideAutoCAD";

    /// <summary>
    /// Relative path from ProgramData to the application bundle installation folder.
    /// </summary>
    /// <remarks>
    /// Value: "Autodesk\ApplicationPlugins\Rhino.Inside.AutoCAD.bundle".
    /// This is the standard AutoCAD plugin installation location.
    /// </remarks>
    public const string RootInstallFolderName =
        "Autodesk\\ApplicationPlugins\\Rhino.Inside.AutoCAD.bundle";

    /// <summary>
    /// Relative path from AppData to the user-specific application data folder.
    /// </summary>
    /// <remarks>
    /// Value: "Bimorph\RhinoInsideAutoCAD". Stores user settings, logs, and cached data.
    /// </remarks>
    public const string ApplicationFolderName = "Bimorph\\RhinoInsideAutoCAD";

    /// <summary>
    /// Prefix used for versioned deployment package names.
    /// </summary>
    /// <remarks>
    /// Value: "RhinoInsideAutoCAD.Applications.". Combined with version number for package naming.
    /// </remarks>
    public const string PackagePrefixName = "RhinoInsideAutoCAD.Applications.";

    /// <summary>
    /// Display name for the Rhino.Inside tab in the AutoCAD ribbon.
    /// </summary>
    public const string RhinoInsideTabName = "Rhino.Inside";

    /// <summary>
    /// Pixel dimension for small ribbon icons (16x16).
    /// </summary>
    /// <seealso cref="LargeIconSize"/>
    public const int SmallIconSize = 16;

    /// <summary>
    /// Pixel dimension for large ribbon icons (32x32).
    /// </summary>
    /// <seealso cref="SmallIconSize"/>
    public const int LargeIconSize = 32;

    /// <summary>
    /// Unique identifier for the Grasshopper preview "Off" button.
    /// </summary>
    /// <seealso cref="ShadedButtonId"/>
    /// <seealso cref="WireframeButtonId"/>
    public const string OffButtonId = "GrasshopperPreviewOffButtonId";

    /// <summary>
    /// Unique identifier for the Grasshopper preview "Shaded" button.
    /// </summary>
    /// <seealso cref="OffButtonId"/>
    /// <seealso cref="WireframeButtonId"/>
    public const string ShadedButtonId = "GrasshopperPreviewShadedButtonId";

    /// <summary>
    /// Unique identifier for the Grasshopper preview "Wireframe" button.
    /// </summary>
    /// <seealso cref="OffButtonId"/>
    /// <seealso cref="ShadedButtonId"/>
    public const string WireframeButtonId = "GrasshopperPreviewWireframeButtonId";

    /// <summary>
    /// Pack URI for the unselected "Off" preview button icon.
    /// </summary>
    /// <seealso cref="OffButtonSelected"/>
    public const string OffButtonUnselected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Off.png";

    /// <summary>
    /// Pack URI for the selected "Off" preview button icon.
    /// </summary>
    /// <seealso cref="OffButtonUnselected"/>
    public const string OffButtonSelected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Off_Selected.png";

    /// <summary>
    /// Pack URI for the unselected "Shaded" preview button icon.
    /// </summary>
    /// <seealso cref="ShadedButtonSelected"/>
    public const string ShadedButtonUnselected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Shaded.png";

    /// <summary>
    /// Pack URI for the selected "Shaded" preview button icon.
    /// </summary>
    /// <seealso cref="ShadedButtonUnselected"/>
    public const string ShadedButtonSelected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Shaded_Selected.png";

    /// <summary>
    /// Pack URI for the unselected "Wireframe" preview button icon.
    /// </summary>
    /// <seealso cref="WireframeButtonSelected"/>
    public const string WireframeButtonUnselected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Wireframe.png";

    /// <summary>
    /// Pack URI for the selected "Wireframe" preview button icon.
    /// </summary>
    /// <seealso cref="WireframeButtonUnselected"/>
    public const string WireframeButtonSelected =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Wireframe_Selected.png";

    /// <summary>
    /// Windows Registry key path, relative to HKEY_LOCAL_MACHINE, under which every
    /// installed Rhino version registers itself.
    /// </summary>
    /// <remarks>
    /// Value: "SOFTWARE\McNeel\Rhinoceros". Each installed version adds a subkey named
    /// after its version, for example "8.0" or "9.0", containing the
    /// <see cref="RhinoInstallSubKeyName"/> subkey. Enumerated by the
    /// <see cref="Core.Interfaces.IRhinoInstallationLocator"/> to discover the versions
    /// available on this machine.
    /// </remarks>
    /// <seealso cref="RhinoInstallSubKeyName"/>
    /// <seealso cref="RhinoInstallPathValueName"/>
    /// <seealso cref="RhinoPluginsFolderValueName"/>
    public const string RhinoRegistryKeyPath = @"SOFTWARE\McNeel\Rhinoceros";

    /// <summary>
    /// Name of the subkey beneath a Rhino version key holding the installation values.
    /// </summary>
    /// <seealso cref="RhinoRegistryKeyPath"/>
    public const string RhinoInstallSubKeyName = "Install";

    /// <summary>
    /// Registry value name for the Rhino installation directory path.
    /// </summary>
    /// <seealso cref="RhinoRegistryKeyPath"/>
    public const string RhinoInstallPathValueName = "Path";

    /// <summary>
    /// Registry value name for the default Rhino plugins folder path.
    /// </summary>
    /// <seealso cref="RhinoRegistryKeyPath"/>
    public const string RhinoPluginsFolderValueName = "Default Plug-ins Folder";

    /// <summary>
    /// Folder beneath the Rhino system directory containing the .NET Core assemblies.
    /// </summary>
    /// <remarks>
    /// Value: "netcore". Rhino ships its .NET Framework assemblies in the system directory
    /// root and its .NET Core assemblies in this subfolder; the NET8 build of this plugin
    /// must bind to the latter.
    /// </remarks>
    public const string RhinoNetCoreFolderName = "netcore";

    /// <summary>
    /// Format string for a Rhino version's display name.
    /// </summary>
    /// <remarks>
    /// Value: "Rhino {0}". The placeholder receives the major version number.
    /// </remarks>
    public const string RhinoDisplayNameFormat = "Rhino {0}";

    /// <summary>
    /// The Rhino major versions this build is able to host.
    /// </summary>
    /// <remarks>
    /// Rhino 9 requires .NET Core, so it is only offered by the NET8 build which runs in
    /// AutoCAD 2025 and later. The NET48 build, which runs in AutoCAD 2024, can only host
    /// Rhino 8.
    /// </remarks>
    public static readonly IReadOnlyList<int> SupportedRhinoMajorVersions =
#if NET8_0_OR_GREATER
        [8, 9];
#else
        [8];
#endif

    /// <summary>
    /// Assembly name for RhinoCommon (without file extension).
    /// </summary>
    /// <remarks>
    /// Used for assembly resolution and binding redirects.
    /// </remarks>
    /// <seealso cref="RhinoCommonDllName"/>
    public const string RhinoCommonAssemblyName = "RhinoCommon";

    /// <summary>
    /// Assembly name for Grasshopper (without file extension).
    /// </summary>
    /// <seealso cref="GrasshopperDllRelativePath"/>
    public const string GrasshopperAssemblyName = "Grasshopper";

    /// <summary>
    /// Assembly name for GH_IO (without file extension).
    /// </summary>
    /// <seealso cref="GrasshopperIoDllRelativePath"/>
    public const string GrasshopperIOAssemblyName = "GH_IO";

    /// <summary>
    /// Filename for the RhinoCommon assembly.
    /// </summary>
    /// <seealso cref="RhinoCommonAssemblyName"/>
    public const string RhinoCommonDllName = "RhinoCommon.dll";

    /// <summary>
    /// Relative path from the plugins folder to Grasshopper.dll.
    /// </summary>
    /// <seealso cref="GrasshopperAssemblyName"/>
    public const string GrasshopperDllRelativePath = "Grasshopper//Grasshopper.dll";

    /// <summary>
    /// Relative path from the plugins folder to GH_IO.dll.
    /// </summary>
    /// <seealso cref="GrasshopperIOAssemblyName"/>
    public const string GrasshopperIoDllRelativePath = "Grasshopper//GH_IO.dll";

    /// <summary>
    /// Command-line argument to suppress the Rhino splash screen on startup.
    /// </summary>
    /// <remarks>
    /// Value: "/nosplash". Used when launching Rhino in embedded mode.
    /// </remarks>
    public const string RhinoNoSplashArgument = "/nosplash";

    /// <summary>
    /// Format string for specifying a Rhino scheme via command-line.
    /// </summary>
    /// <remarks>
    /// Value: "/scheme={0}". The placeholder receives the scheme name.
    /// </remarks>
    /// <seealso cref="RhinoInsideSchemeNameFormat"/>
    public const string RhinoSchemeArgumentFormat = "/scheme={0}";

    /// <summary>
    /// Format string for generating the Rhino.Inside scheme name.
    /// </summary>
    /// <remarks>
    /// Value: "Inside-{0}-{1}". Placeholders typically receive host application name and version.
    /// </remarks>
    /// <seealso cref="RhinoSchemeArgumentFormat"/>
    public const string RhinoInsideSchemeNameFormat = "Inside-{0}-{1}";

    /// <summary>
    /// Error message displayed when no supported Rhino version is detected on the system.
    /// </summary>
    /// <seealso cref="RhinoRegistryKeyPath"/>
    /// <seealso cref="SupportedRhinoMajorVersions"/>
    public const string RhinoNotInstalledErrorMessage =
        "No supported version of Rhino was found. Rhino.Inside.AutoCAD requires Rhino 8 or later to run.";

    /// <summary>
    /// Error message recorded when the user cancels the Rhino version selection dialog.
    /// </summary>
    /// <remarks>
    /// Rhino is not loaded for the remainder of the session. The choice is made once per
    /// AutoCAD session because the assembly resolvers it drives cannot be re-registered,
    /// so the user is directed to the settings page and a restart.
    /// </remarks>
    public const string RhinoVersionNotSelectedErrorMessage =
        "Rhino.Inside.AutoCAD did not start because no Rhino version was selected. " +
        "Restart AutoCAD to choose one.";

    /// <summary>
    /// Format string for the command line message written when the plugin declines to load.
    /// </summary>
    /// <remarks>
    /// Value: "\nRhino.Inside.AutoCAD did not load: {0}\n". The placeholder receives the reason.
    /// </remarks>
    public const string ApplicationLoadAbortedMessageFormat =
        "\nRhino.Inside.AutoCAD did not load: {0}\n";

    /// <summary>
    /// Error message displayed when RhinoCore initialization fails.
    /// </summary>
    public const string RhinoCoreInitializationFailedErrorMessage =
        "Failed to initialize Rhino Core";

    /// <summary>
    /// Success message written to the AutoCAD command line after successful initialization.
    /// </summary>
    public const string ApplicationLoadedSuccessMessage =
        "\nRhino.Inside.AutoCAD loaded successfully.";

    /// <summary>
    /// Format string for error messages during application loading.
    /// </summary>
    /// <remarks>
    /// Value: "\nERROR loading Rhino.Inside.AutoCAD: {0}\n". Placeholder receives exception message.
    /// </remarks>
    /// <seealso cref="StackTraceMessageFormat"/>
    public const string ApplicationLoadErrorMessageFormat =
        "\nERROR loading Rhino.Inside.AutoCAD: {0}\n";

    /// <summary>
    /// Format string for displaying exception stack traces.
    /// </summary>
    /// <remarks>
    /// Value: "\nStack trace: {0}\n". Placeholder receives the stack trace string.
    /// </remarks>
    /// <seealso cref="ApplicationLoadErrorMessageFormat"/>
    public const string StackTraceMessageFormat = "\nStack trace: {0}\n";

    /// <summary>
    /// Message displayed when the application license has expired.
    /// </summary>
    /// <remarks>
    /// Includes a URL directing users to download an updated version.
    /// </remarks>
    public const string ExpiredMessage =
        "This version of Rhino.Inside.AutoCAD has expired. Please download the latest version.";

    /// <summary>
    /// URL to download the latest version of Rhino.Inside.AutoCAD.
    /// </summary>
    public const string DownloadUrl = "https://www.bimorph.com/products/rhino-inside-autocad";

    /// <summary>
    /// Error message displayed when the RhinoDoc fails to initialize.
    /// </summary>
    public const string FailedToLoadRhinoDoc = "Failed to initialize Rhino Doc.";

    /// <summary>
    /// Diagnostic recorded when a new Rhino document cannot be given the AutoCAD document's
    /// units. The placeholder receives the unit system that was refused.
    /// </summary>
    /// <remarks>
    /// Never shown to the user: the document is still usable in the template's units, and the
    /// units are theirs to change.
    /// </remarks>
    public const string FailedToMatchAutoCadUnits =
        "Failed to set the new Rhino document's units to the AutoCAD document's units ({0}). " +
        "The template's units are kept.";

    /// <summary>
    /// Prefix used to denote build metadata in semantic version strings.
    /// </summary>
    /// <remarks>
    /// Value: "+build". Follows SemVer 2.0 specification for build metadata.
    /// </remarks>
    public const string BuildVersionMetadataPrefix = "+build";

    /// <summary>
    /// Unique identifier for the Rhino object preview toggle button.
    /// </summary>
    /// <seealso cref="RhinocerosPreviewShadedIcon"/>
    /// <seealso cref="RhinocerosPreviewOffIcon"/>
    public const string RhinoPreviewButtonId = "RhinoPreviewButtonId";

    /// <summary>
    /// Unique identifier for the Grasshopper solver toggle button.
    /// </summary>
    /// <seealso cref="GrasshopperSolverOnIcon"/>
    /// <seealso cref="GrasshopperSolverOffIcon"/>
    public const string GrasshopperSolverButtonId = "GrasshopperSolverButtonId";

    /// <summary>
    /// Pack URI for the Rhino preview shaded (enabled) icon.
    /// </summary>
    /// <seealso cref="RhinocerosPreviewOffIcon"/>
    public const string RhinocerosPreviewShadedIcon =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Rhinoceros_Preview_Shaded.png";

    /// <summary>
    /// Pack URI for the Rhino preview off (disabled) icon.
    /// </summary>
    /// <seealso cref="RhinocerosPreviewShadedIcon"/>
    public const string RhinocerosPreviewOffIcon =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Rhinoceros_Preview_Off.png";

    /// <summary>
    /// Pack URI for the Grasshopper solver enabled icon.
    /// </summary>
    /// <seealso cref="GrasshopperSolverOffIcon"/>
    public const string GrasshopperSolverOnIcon =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_SolverOn.png";

    /// <summary>
    /// Pack URI for the Grasshopper solver disabled icon.
    /// </summary>
    /// <seealso cref="GrasshopperSolverOnIcon"/>
    public const string GrasshopperSolverOffIcon =
        "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_SolverOff.png";

    /// <summary>
    /// Rhino command name that launches the Grasshopper editor.
    /// </summary>
    public const string GrasshopperCommandName = "Grasshopper";

    /// <summary>
    /// Rhino command name that opens the Package Manager.
    /// </summary>
    public const string PackageManagerCommandName = "PackageManager";

    /// <summary>
    /// Rhino command name that launches Grasshopper Player.
    /// </summary>
    public const string GrasshopperPlayerCommandName = "GrasshopperPlayer";

    /// <summary>
    /// Rhino script macro for creating a new floating viewport with copied projection.
    /// </summary>
    /// <remarks>
    /// Value: "_NewFloatingViewport _Projection _CopyActive".
    /// </remarks>
    public const string NewFloatingViewportScript =
        "_NewFloatingViewport _Projection _CopyActive";

    /// <summary>
    /// Format string for the default Rhino template file path.
    /// </summary>
    /// <remarks>
    /// Value: "{0}Large Objects - Millimeters.3dm". Placeholder receives the templates
    /// directory path.
    /// </remarks>
    public const string DefaultTemplateFormat = "{0}Large Objects - Millimeters.3dm";

    /// <summary>
    /// The family version of the System.ServiceModel.Primitive assemblies for .NET 8. Corresponding
    /// to the version shipped with each version of AutoCAD.
    /// </summary>
    public const string SystemPrimitiveDll = "System.ServiceModel.Primitives.{0}.dll";

    /// <summary>
    /// The family version of the System.ServiceModel.Primitive assemblies for .NET 8. Corresponding
    /// to the version shipped with each version of AutoCAD.
    /// </summary>
    public const string SystemHttpDll = "System.ServiceModel.Http.{0}.dll";

    /// <summary>
    /// The family version of the System.ServiceModel assemblies for Later Autocad 2025.
    /// </summary>
    public const string ServiceModelFamliy8_0 = "8.0";

    /// <summary>
    /// The family version of the System.ServiceModel assemblies for Earlier Autocad 2025.
    /// </summary>
    public const string ServiceModelFamliy6_0 = "6.0";

    /// <summary>
    /// The family version of the System.ServiceModel assemblies for Autocad 2026.
    /// </summary>
    public const string ServiceModelFamliy8_1 = "8.1";
}
