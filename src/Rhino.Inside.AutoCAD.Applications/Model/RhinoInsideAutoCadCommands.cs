using Autodesk.AutoCAD.Runtime;
using Bimorph.Core.Services.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
using Rhino.Inside.AutoCAD.GrasshopperLauncher;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.RhinoLauncher;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using CADApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: CommandClass(typeof(RhinoInsideAutoCadCommands))]

namespace Rhino.Inside.AutoCAD.Applications;

public class RhinoInsideAutoCadCommands
{
    private static UnitSystem InternalUnitSystem => InteropConstants.InternalUnitSystem;
    private static bool _isRunning;

    /// <summary>
    /// The command to launch a GUI application with Rhino Inside.
    /// </summary>
    private static void RunApplicationWithRhino(
        Func<IRhinoInsideAutoCadApplication, IInteropService, IApplicationMain> mainlineType,
        ButtonApplicationId appId, RhinoInsideMode mode)
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

            var rhinoInstance = RhinoInsideExtension.Instance;

            var validationLogger = rhinoInstance.ValidationLogger;

            if (validationLogger.HasValidationErrors)
            {
                CADApplication.ShowAlertDialog(validationLogger.GetMessage());

                return;
            }

            rhinoInstance.Initialize(InternalUnitSystem, fileResourceManager.ApplicationDirectories, mode);

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
        RunApplicationWithRhino((application, interopService) =>
            new RhinoLauncherMain(application, interopService), ButtonApplicationId.RHINO, RhinoInsideMode.Windowed);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER", CommandFlags.Modal)]
    public static void GRASSHOPPER()
    {
        RunApplicationWithRhino((application, interopService) =>
            new GrasshopperLauncherMain(application, interopService), ButtonApplicationId.GRASSHOPPER, RhinoInsideMode.Headless);
    }
}
