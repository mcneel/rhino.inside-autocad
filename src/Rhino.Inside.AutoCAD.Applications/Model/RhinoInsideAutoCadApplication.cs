using Autodesk.AutoCAD.ApplicationServices.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Services;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoInsideAutoCadApplication"/>
public class RhinoInsideAutoCadApplication : IRhinoInsideAutoCadApplication
{
    private readonly IList<string> _materialDesignAssemblyNames = ApplicationConstants.MaterialDesignAssemblyNames;

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
    public IRhinoInsideManager RhinoInsideManager { get; }

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

        var rhinoInstance = new RhinoInstance(applicationDirectories);

        var grasshopperInstance = new GrasshopperInstance(applicationDirectories);

        var autocadInstance = new AutoCadInstance(bootstrapper.Dispatcher);

        var rhinoInsideManager = new RhinoInsideManager(rhinoInstance, grasshopperInstance, autocadInstance);

        this.SettingsManager = settingManager;

        this.FileResourceManager = fileResourceManager;

        this.Bootstrapper = bootstrapper;

        this.ApplicationServicesCore = applicationServicesCore;

        this.ApplicationConfig = applicationConfig;

        this.RhinoInsideManager = rhinoInsideManager;

        this.LoadMaterialDesign(applicationDirectories);
    }

    /// <summary>
    /// The Material Design library has to be force loaded into Revit to avoid runtime
    /// exceptions as it's not automatically loaded as the calls to the library are always
    /// from XAML. This method guarantees its loaded.
    /// </summary>
    private void LoadMaterialDesign(IApplicationDirectories applicationDirectories)
    {
        foreach (var names in _materialDesignAssemblyNames)
        {
            var assemblyPath = Path.Combine(applicationDirectories.Assemblies, names);
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

            Assembly.Load(assemblyName);
        }
    }

    /// <inheritdoc />
    public void ShowAlertDialog(string message)
    {
        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(message);
    }

    /// <inheritdoc />
    public void Terminate()
    {
        try
        {
            this.Bootstrapper?.AssemblyManager.ShutDown();

            RhinoCoreExtension.Instance.Shutdown();

        }
        catch (System.Exception e)
        {
            LoggerService.Instance?.LogError(e);
        }
    }
}
