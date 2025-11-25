using AWI.SectionDetailsTool;
using Bimorph.Core.Services.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino;
using Rhino.Inside.AutoCAD.Core.Enum.Application;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using CADApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using CommandClass = Autodesk.AutoCAD.Runtime.CommandClassAttribute;
using CommandMethod = Autodesk.AutoCAD.Runtime.CommandMethodAttribute;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: CommandClass(typeof(AWI.Applications.RhinoInsideAutoCadCommands))]

namespace AWI.Applications;

public class RhinoInsideAutoCadCommands
{
    private static readonly UnitSystem _internalUnitSystem = ApplicationConstants.InternalUnitSystem;
    private static bool _isRunning;

    /// <summary>
    /// The command to launch a GUI AWI application.
    /// </summary>
    private static void RunApplication(
        Func<IRhinoInsideAutoCadApplication, IApplicationMain> mainlineType,
        RhinoInsideApplicationId appId)
    {
        if (_isRunning)
            return;

        var application = new RhinoInsideAutoCadApplication();

        var splashScreenLauncher = new SplashScreenLauncher(application);

        splashScreenLauncher.Show();

        var applicationServicesCore = application.ApplicationServicesCore;

        var fileResourceManager = application.FileResourceManager;

        var bootstrapper = application.Bootstrapper;

        var dispatcher = bootstrapper.Dispatcher;

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

            rhinoInstance.Initialize(_internalUnitSystem, fileResourceManager.ApplicationDirectories);

            var interopService = new InteropService(dispatcher, fileResourceManager, appId);

            var mainline = mainlineType(application);

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

    [CommandMethod("Rhino")]
    public static void RHINOINSIDEAUTOCAD_RHINO()
    {
        RunApplication((application) =>
            new RhinoLauncherMain(application), RhinoInsideApplicationId.Rhino);
    }

    [CommandMethod("Grasshopper")]
    public static void RHINOINSIDEAUTOCAD_GRASSHOPPER()
    {
        RunApplication((application) =>
            new GrasshopperLauncherMain(application), RhinoInsideApplicationId.Grasshopper);
    }
}
