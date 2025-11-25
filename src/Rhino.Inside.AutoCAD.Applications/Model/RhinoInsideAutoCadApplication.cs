using Autodesk.AutoCAD.ApplicationServices.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IRhinoInsideAutoCadApplication"/>
public class RhinoInsideAutoCadApplication : IRhinoInsideAutoCadApplication
{
    /// <inheritdoc/>
    public IRhinoInsideAutoCadSettingsManager SettingsManager { get; }

    /// <inheritdoc/>
    public IRhinoInsideAutoCadFileResourceManager FileResourceManager { get; }

    /// <inheritdoc/>
    public IBootstrapper Bootstrapper { get; }

    /// <inheritdoc/>
    public IApplicationServicesCore ApplicationServicesCore { get; }

    /// <inheritdoc/>
    public IApplicationConfig ApplicationConfig { get; }

    /// <inheritdoc/>
    public ISupportedApplicationManager SupportedApplicationManager { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public RhinoInsideAutoCadApplication()
    {
        var applicationConfig = new RhinoInsideAutoCadApplicationConfig();

        var bootstrapConfig = new AutocadBootstrapperConfig(Application.MainWindow.Handle, applicationConfig);

        var bootstrapper = new Bootstrapper(bootstrapConfig);

        var applicationDirectories = bootstrapper.ApplicationDirectories;

        var settingManager = new SettingManager(applicationDirectories);

        var fileResourceManager = new FileResourceManager(applicationDirectories, settingManager);

        SoftwareUpdater.Initialize(bootstrapper, applicationConfig, settingManager);

        var applicationServicesCore = new ApplicationServicesCore();

        this.SettingsManager = settingManager;

        this.FileResourceManager = fileResourceManager;

        this.Bootstrapper = bootstrapper;

        this.ApplicationServicesCore = applicationServicesCore;

        this.ApplicationConfig = applicationConfig;

        this.SupportedApplicationManager = bootstrapper.SupportedApplicationManager;


    }
}
