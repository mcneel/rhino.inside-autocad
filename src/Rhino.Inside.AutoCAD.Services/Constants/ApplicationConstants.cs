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
}