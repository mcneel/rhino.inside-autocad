using Autofac;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;

namespace Rhino.Inside.AutoCAD.GrasshopperLauncher;

/// The Entry point to the rhino launcher application.
/// <inheritdoc cref="ApplicationMainBase"/>
public class GrasshopperLauncherMain : ApplicationMainBase
{
    private readonly IRhinoInsideAutoCadApplication _application;
    private readonly IInteropService _interopService;
    private readonly IRhinoInsideAutoCadFileResourceManager _fileResourceManager;
    private readonly IRhinoInsideAutoCadSettingsManager _settingsManager;

    /// <summary>
    /// Constructs a new <see cref="GrasshopperLauncherMain"/>.
    /// </summary>
    public GrasshopperLauncherMain(IRhinoInsideAutoCadApplication application, IInteropService interopService)
        : base(application)
    {
        _application = application;

        _interopService = interopService;

        _fileResourceManager = _application.FileResourceManager;

        _settingsManager = _application.SettingsManager;
    }

    ///<inheritdoc />
    protected override void RegisterTypes()
    {
        _containerBuilder.RegisterInstance(_application)
            .As<IRhinoInsideAutoCadApplication>()
            .As<IBimorphApplication>()
            .SingleInstance();
        _containerBuilder.RegisterInstance(_interopService).As<IInteropService>().SingleInstance();
        _containerBuilder.RegisterInstance(_settingsManager).As<IRhinoInsideAutoCadSettingsManager>().SingleInstance();
        _containerBuilder.RegisterInstance(_fileResourceManager.ApplicationDirectories).As<IApplicationDirectories>().SingleInstance();
        _containerBuilder.RegisterInstance(_fileResourceManager)
            .As<IRhinoInsideAutoCadFileResourceManager>()
            .As<IFileResourceManager>()
            .SingleInstance();
    }
}
