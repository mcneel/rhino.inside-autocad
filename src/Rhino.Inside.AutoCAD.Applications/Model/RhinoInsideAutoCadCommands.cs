using Autodesk.AutoCAD.Runtime;
using Bimorph.Core.Services.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
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

        var rhinoInstance = application.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand("Grasshopper");
    }
}