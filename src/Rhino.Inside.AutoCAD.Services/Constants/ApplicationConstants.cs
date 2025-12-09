namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// An example constants class for an application.
/// </summary>
public class ApplicationConstants
{
    /// <summary>
    /// The name of the application. This must match the Product name in the assembly
    /// info (Build.Props).
    /// </summary>
    public const string ProductName = "Rhino.Inside.AutoCAD";

    /// <summary>
    /// The name of the application.
    /// </summary>
    public const string ApplicationName = "RhinoInsideAutoCAD";

    /// <summary>
    /// The name of the root installation folder the software is stored.
    /// </summary>
    public const string RootInstallFolderName = "Autodesk\\ApplicationPlugins\\Rhino.Inside.AutoCAD.bundle";

    /// <summary>
    /// The name of the client folder.
    /// </summary>
    public const string ClientFolderName = "Bimorph\\RhinoInsideAutoCAD";

    /// <summary>
    /// The name that is prefixed to the version number of the deployment.
    /// </summary>
    public const string PackagePrefixName = "RhinoInsideAutoCAD.Applications.";

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
    /// The name  of the Rhino.Inside tab in the AutoCad application ribbon.
    /// </summary>
    public const string RhinoInsideTabName = "Rhino.Inside";

    /// <summary>
    /// The size of small icons used in the AutoCad application ribbon and UI.
    /// </summary>
    public const int SmallIconSize = 16;

    /// <summary>
    /// The size of small icons used in the AutoCad application ribbon and UI.
    /// </summary>
    public const int LargeIconSize = 32;

    /// <summary>
    /// The identifier for the "Off" button in the Grasshopper preview.
    /// </summary>
    public const string OffButtonId = "GrasshopperPreviewOffButtonId";

    /// <summary>
    /// The identifier for the "Shaded" button in the Grasshopper preview.
    /// </summary>
    public const string ShadedButtonId = "GrasshopperPreviewShadedButtonId";

    /// <summary>
    /// The identifier for the "Wireframe" button in the Grasshopper preview.
    /// </summary>
    public const string WireframeButtonId = "GrasshopperPreviewWireframeButtonId";

    /// <summary>
    /// The URI for the unselected "Off" button icon in the Grasshopper preview.
    /// </summary>
    public const string OffButtonUnselected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Off.png";

    /// <summary>
    /// The URI for the selected "Off" button icon in the Grasshopper preview.
    /// </summary>
    public const string OffButtonSelected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Off_Selected.png";

    /// <summary>
    /// The URI for the unselected "Shaded" button icon in the Grasshopper preview.
    /// </summary>
    public const string ShadedButtonUnselected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Shaded.png";

    /// <summary>
    /// The URI for the selected "Shaded" button icon in the Grasshopper preview.
    /// </summary>
    public const string ShadedButtonSelected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Shaded_Selected.png";

    /// <summary>
    /// The URI for the unselected "Wireframe" button icon in the Grasshopper preview.
    /// </summary>
    public const string WireframeButtonUnselected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Wireframe.png";

    /// <summary>
    /// The URI for the selected "Wireframe" button icon in the Grasshopper preview.
    /// </summary>
    public const string WireframeButtonSelected = "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Grasshopper_Preview_Wireframe_Selected.png";

    #region Rhino Constants

    /// <summary>
    /// The registry key path for Rhino 8 installation information.
    /// </summary>
    public const string RhinoRegistryKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\8.0\Install";

    /// <summary>
    /// The registry value name for Rhino installation path.
    /// </summary>
    public const string RhinoInstallPathValueName = "Path";

    /// <summary>
    /// The registry value name for Rhino default plugins folder.
    /// </summary>
    public const string RhinoPluginsFolderValueName = "Default Plug-ins Folder";

    /// <summary>
    /// The name of the RhinoCommon assembly (without extension).
    /// </summary>
    public const string RhinoCommonAssemblyName = "RhinoCommon";

    /// <summary>
    /// The name of the Grasshopper assembly (without extension).
    /// </summary>
    public const string GrasshopperAssemblyName = "Grasshopper";

    /// <summary>
    /// The filename of the RhinoCommon assembly.
    /// </summary>
    public const string RhinoCommonDllName = "RhinoCommon.dll";

    /// <summary>
    /// The relative path to Grasshopper.dll within the plugins folder.
    /// </summary>
    public const string GrasshopperDllRelativePath = "Grasshopper//Grasshopper.dll";

    /// <summary>
    /// Command line argument to suppress Rhino splash screen.
    /// </summary>
    public const string RhinoNoSplashArgument = "/nosplash";

    /// <summary>
    /// Format string for Rhino scheme name argument.
    /// </summary>
    public const string RhinoSchemeArgumentFormat = "/scheme={0}";

    /// <summary>
    /// Format string for creating Rhino Inside scheme name.
    /// </summary>
    public const string RhinoInsideSchemeNameFormat = "Inside-{0}-{1}";

    /// <summary>
    /// Error message when Rhino 8 is not installed.
    /// </summary>
    public const string RhinoNotInstalledErrorMessage = "Rhino 8 not installed or could not be found. The application requires Rhino 8 to run.";

    /// <summary>
    /// Error message when Rhino Core fails to initialize.
    /// </summary>
    public const string RhinoCoreInitializationFailedErrorMessage = "Failed to initialize Rhino Core";

    #endregion

    #region Application Initialization Messages

    /// <summary>
    /// Success message displayed when Rhino.Inside.AutoCAD loads successfully.
    /// </summary>
    public const string ApplicationLoadedSuccessMessage = "\nRhino.Inside.AutoCAD loaded successfully.";

    /// <summary>
    /// Format string for error message when Rhino.Inside.AutoCAD fails to load.
    /// </summary>
    public const string ApplicationLoadErrorMessageFormat = "\nERROR loading Rhino.Inside.AutoCAD: {0}\n";

    /// <summary>
    /// Format string for displaying stack trace information.
    /// </summary>
    public const string StackTraceMessageFormat = "\nStack trace: {0}\n";

    #endregion
}