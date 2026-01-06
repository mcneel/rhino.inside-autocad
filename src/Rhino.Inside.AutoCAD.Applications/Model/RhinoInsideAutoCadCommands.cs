using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Services;

[assembly: CommandClass(typeof(RhinoInsideAutoCadCommands))]

namespace Rhino.Inside.AutoCAD.Applications;

/// <summary>
/// The commands class for Rhino.Inside.AutoCAD application commands. This class contains
/// all the command methods that can be invoked from AutoCAD to interact with Rhino and Grasshopper.
/// </summary>
public class RhinoInsideAutoCadCommands
{
    private static bool _isLaunching;

    private const string _rhinoPreviewButtonId = ApplicationConstants.RhinoPreviewButtonId;
    private const string _grasshopperSolverButtonId = ApplicationConstants.GrasshopperSolverButtonId;
    private const string _rhinocerosPreviewShadedIcon = ApplicationConstants.RhinocerosPreviewShadedIcon;
    private const string _rhinocerosPreviewOffIcon = ApplicationConstants.RhinocerosPreviewOffIcon;
    private const string _grasshopperSolverOnIcon = ApplicationConstants.GrasshopperSolverOnIcon;
    private const string _grasshopperSolverOffIcon = ApplicationConstants.GrasshopperSolverOffIcon;
    private const string _grasshopperCommandName = ApplicationConstants.GrasshopperCommandName;
    private const string _packageManagerCommandName = ApplicationConstants.PackageManagerCommandName;
    private const string _grasshopperPlayerCommandName = ApplicationConstants.GrasshopperPlayerCommandName;
    private const string _newFloatingViewportScript = ApplicationConstants.NewFloatingViewportScript;
    private const string _expiredMessage = ApplicationConstants.ExpiredMessage;

    /// <summary>
    /// Checks if the application has expired and shows an alert dialog if it has.
    /// </summary>
    private static bool CheckApplicationHasExpired()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        if (application is null)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(_expiredMessage);
            return true;
        }

        return false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO", CommandFlags.Modal)]
    public static void RHINO()
    {
        if (_isLaunching || CheckApplicationHasExpired())
            return;

        _isLaunching = true;
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Windowed);

        _isLaunching = false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER", CommandFlags.Modal)]
    public static void GRASSHOPPER()
    {
        if (_isLaunching || CheckApplicationHasExpired())
            return;

        _isLaunching = true;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand(_grasshopperCommandName);
        _isLaunching = false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "TOGGLE_RHINO_PREVIEW", CommandFlags.Modal)]
    public static void TOGGLE_RHINO_PREVIEW()
    {

        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        var rhinoObjectPreview = rhinoInsideManager.RhinoPreviewServer;

        rhinoObjectPreview.ToggleVisibility();

        var buttonReplacer = new ButtonIconReplacer(_rhinoPreviewButtonId);

        var imagePath = rhinoObjectPreview.Visible
            ? _rhinocerosPreviewShadedIcon
            : _rhinocerosPreviewOffIcon;

        buttonReplacer.Replace(imagePath);

    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_PREVIEW_OFF", CommandFlags.Modal)]
    public static void GRASSHOPPER_PREVIEW_OFF()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        var grasshopperPreview = rhinoInsideManager.GrasshopperPreviewServer;
        grasshopperPreview.SetMode(GrasshopperPreviewMode.Off);

    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_PREVIEW_SHADED", CommandFlags.Modal)]
    public static void GRASSHOPPER_PREVIEW_SHADED()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        var grasshopperPreview = rhinoInsideManager.GrasshopperPreviewServer;
        grasshopperPreview.SetMode(GrasshopperPreviewMode.Shaded);

    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_PREVIEW_WIREFRAME", CommandFlags.Modal)]
    public static void GRASSHOPPER_PREVIEW_WIREFRAME()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        var grasshopperPreview = rhinoInsideManager.GrasshopperPreviewServer;
        grasshopperPreview.SetMode(GrasshopperPreviewMode.Wireframe);

    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_RECOMPUTE", CommandFlags.Modal)]
    public static void GRASSHOPPER_RECOMPUTE()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        if (rhinoInsideManager.RhinoInstance.ActiveDoc == null) return;

        var grasshopperInstance = rhinoInsideManager.GrasshopperInstance;

        grasshopperInstance.RecomputeSolution();
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_TOGGLE_SOLVER", CommandFlags.Modal)]
    public static void GRASSHOPPER_TOGGLE_SOLVER()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        if (rhinoInsideManager.RhinoInstance.ActiveDoc == null) return;

        var grasshopperInstance = rhinoInsideManager.GrasshopperInstance;

        var isEnabled = grasshopperInstance.IsEnabled;

        if (isEnabled)
        {
            grasshopperInstance.DisableSolver();
        }
        else
        {
            grasshopperInstance.EnableSolver();
        }

        var buttonReplacer = new ButtonIconReplacer(_grasshopperSolverButtonId);

        var imagePath = isEnabled
            ? _grasshopperSolverOffIcon
            : _grasshopperSolverOnIcon;

        buttonReplacer.Replace(imagePath);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "OPEN_RHINO_VIEWPORT", CommandFlags.Modal)]
    public static void OPEN_RHINO_VIEWPORT()
    {
        if (_isLaunching || CheckApplicationHasExpired())
            return;

        _isLaunching = true;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoScript(_newFloatingViewportScript);

        _isLaunching = false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_PACKAGE_MANGER", CommandFlags.Modal)]
    public static void RHINO_PACKAGE_MANGER()
    {
        if (_isLaunching || CheckApplicationHasExpired())
            return;

        _isLaunching = true;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand(_packageManagerCommandName);

        _isLaunching = false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_PLAYER", CommandFlags.Modal)]
    public static void GRASSHOPPER_PLAYER()
    {
        if (_isLaunching || CheckApplicationHasExpired())
            return;

        _isLaunching = true;

        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand(_grasshopperPlayerCommandName);

        _isLaunching = false;
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_INSIDE_ABOUT", CommandFlags.Modal)]
    public static void RHINO_INSIDE_ABOUT()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        application.SupportDialogManager.Show(SupportDialogTab.About);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_INSIDE_SUPPORT", CommandFlags.Modal)]
    public static void RHINO_INSIDE_SUPPORT()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        application.SupportDialogManager.Show(SupportDialogTab.Support);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_INSIDE_UPDATE", CommandFlags.Modal)]
    public static void RHINO_INSIDE_UPDATE()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        application.SupportDialogManager.Show(SupportDialogTab.Update);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_INSIDE_CONVERT_BREP", CommandFlags.Transparent)]
    public static void RHINO_INSIDE_CONVERT_BREP()
    {
        if (CheckApplicationHasExpired())
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var brepConverterRunner = application.BrepConverterRunner;

        brepConverterRunner.Run();

    }
}
