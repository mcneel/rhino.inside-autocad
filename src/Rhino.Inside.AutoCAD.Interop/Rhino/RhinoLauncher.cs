using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoLauncher"/>
public class RhinoLauncher : IRhinoLauncher
{
    private readonly IRhinoInsideAutoCadApplication _application;
    private readonly IRhinoInsideManager _rhinoInsideManager;

    /// <summary>
    /// Creates a new <see cref="IRhinoLauncher"/> instance.
    /// </summary>
    public RhinoLauncher(IRhinoInsideAutoCadApplication application)
    {
        _application = application;
        _rhinoInsideManager = application.RhinoInsideManager;
    }

    /// <inheritdoc />
    public void Launch(RhinoInsideMode mode)
    {
        //ToDo : Splash Screen then uncomment
        //var application = RhinoInsideAutoCadExtension.Application;

        // var splashScreenLauncher = new SplashScreenLauncher(application);

        // splashScreenLauncher.Show();

        try
        {
            var rhinoCoreExtension = RhinoCoreExtension.Instance;

            var validationLogger = rhinoCoreExtension.ValidationLogger;

            if (validationLogger.HasValidationErrors)
            {
                _application.ShowAlertDialog(validationLogger.GetMessage());

                return;
            }

            rhinoCoreExtension.ValidateRhinoCore();

            var rhinoInstance = _rhinoInsideManager.RhinoInstance;

            rhinoInstance.ValidateRhinoDoc(mode, validationLogger);

            if (mode != RhinoInsideMode.Headless)
            {
                rhinoCoreExtension.WindowManager.ShowWindow();
            }

            //  splashScreenLauncher.Close();
        }
        catch (Exception e)
        {
            // splashScreenLauncher.ShowExceptionInfo();

            LoggerService.Instance.LogError(e);
        }
    }
}