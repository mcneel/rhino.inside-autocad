using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.Models;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoLauncher"/>
public class RhinoLauncher : IRhinoLauncher
{
    private readonly IRhinoInsideAutoCadApplication _application;
    private readonly IRhinoInsideManager _rhinoInsideManager;

    private const string _rhinoStartFailureMessage =
        MessageConstants.RhinoStartFailureMessage;
    private const string _grasshopperCommandName = ApplicationConstants.GrasshopperCommandName;
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
        var loadingScreenLauncher = new LoadingScreenManager(_application);

        loadingScreenLauncher.Show();

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

            var grasshopperInstance = _rhinoInsideManager.GrasshopperInstance;
            grasshopperInstance.ValidateGrasshopperLibrary(validationLogger);

            if (mode != RhinoInsideMode.Headless)
            {
                rhinoCoreExtension.WindowManager.ShowWindow();
            }

            loadingScreenLauncher.Close();
        }
        catch (Exception e)
        {
            loadingScreenLauncher.ShowFailureMessage(string.Format(_rhinoStartFailureMessage, e.Message));

            LoggerService.Instance.LogError(e);
        }
    }
}