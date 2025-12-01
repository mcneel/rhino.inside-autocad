using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using System.Diagnostics;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: CommandClass(typeof(RhinoInsideAutoCadCommands))]

namespace Rhino.Inside.AutoCAD.Applications;

public class RhinoInsideAutoCadCommands
{
    private static bool _isRunning;

    /// <summary>
    /// The command to launch a GUI application with Rhino Inside.
    /// </summary>
    private static void RunApplication(
        Func<IRhinoInsideAutoCadApplication, IInteropService, IApplicationMain> mainlineType,
        ButtonApplicationId appId)
    {
        if (_isRunning)
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var splashScreenLauncher = new SplashScreenLauncher(application);

        splashScreenLauncher.Show();

        var applicationServicesCore = application.ApplicationServicesCore;

        var fileResourceManager = application.FileResourceManager;

        try
        {
            _isRunning = true;

            var interopService = new InteropService(application, appId);

            var mainline = mainlineType(application, interopService);

            mainline.ShutdownStarted += (_, _) => _isRunning = false;

            var result = mainline.Run();

            if (result == RunResult.Invalid)
            {
                var failingService = applicationServicesCore.GetFailedService();

                splashScreenLauncher.ShowFailedValidationInfo(failingService.ValidationLogger);

                _isRunning = false;
            }
            else
            {
                splashScreenLauncher.Close();
            }
        }
        catch (Exception e)
        {
            splashScreenLauncher.ShowExceptionInfo();

            _isRunning = false;

            LoggerService.Instance.LogError(e);
        }
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO", CommandFlags.Modal)]
    public static void RHINO()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Windowed);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER", CommandFlags.Modal)]
    public static void GRASSHOPPER()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand("Grasshopper");
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "TOGGLE_RHINO_PREVIEW", CommandFlags.Modal)]
    public static void TOGGLE_RHINO_PREVIEW()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManager = application!.RhinoInsideManager;

        var rhinoObjectPreview = rhinoInsideManager.AutoCadInstance.RhinoObjectPreviewer;

        rhinoObjectPreview.ToggleVisibility();

        var buttonReplacer = new ButtonIconReplacer("RhinoPreviewButtonId");

        var imagePath = rhinoObjectPreview.Visible
            ? "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Rhinoceros_Preview_Shaded.png"
            : "pack://application:,,,/Rhino.Inside.AutoCAD.Applications;component/Icons/Large512/Rhinoceros_Preview_Off.png";

        buttonReplacer.Replace(imagePath);

    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "OPEN_RHINO_VIEWPORT", CommandFlags.Modal)]
    public static void OPEN_RHINO_VIEWPORT()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoScript("_NewFloatingViewport _Projection _CopyActive");
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO_PACKAGE_MANGER", CommandFlags.Modal)]
    public static void RHINO_PACKAGE_MANGER()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand("PackageManager");
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER_PLAYER", CommandFlags.Modal)]
    public static void GRASSHOPPER_PLAYER()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application!.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand("GrasshopperPlayer");
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "REQUEST_PLUGIN", CommandFlags.Modal)]
    public static void REQUEST_PLUGIN()
    {
        //TODO: These should open a UI first.
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.bimorph.com/contact",
            UseShellExecute = true
        });
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "REQUEST_MISSING_FEATURE", CommandFlags.Modal)]
    public static void REQUEST_MISSING_FEATURE()
    {
        //TODO: These should open a UI first.
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.bimorph.com/contact",
            UseShellExecute = true
        });
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "BIMORPH_WEBSITE", CommandFlags.Modal)]
    public static void BIMORPH_WEBSITE()
    {
        //TODO: These should open a UI first.
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.bimorph.com/",
            UseShellExecute = true
        });
    }
}

