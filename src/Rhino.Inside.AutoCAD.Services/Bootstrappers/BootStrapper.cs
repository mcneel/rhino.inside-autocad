using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class responsible for initializing the base resources for an external application.
/// </summary>
public class Bootstrapper : IBootstrapper
{
    /// <inheritdoc/>
    public Dispatcher Dispatcher { get; }

    /// <inheritdoc/>
    public IAssemblyManager AssemblyManager { get; }

    /// <inheritdoc/>
    public IWindowConfig WindowConfig { get; }

    /// <inheritdoc/>
    public IVersionLog VersionLog { get; }

    /// <inheritdoc/>
    public IApplicationDirectories ApplicationDirectories { get; }

    /// <inheritdoc/>
    public IBootstrapperConfig BootstrapperConfig { get; }

    /// <summary>
    /// Constructs a new application <see cref="Bootstrapper"/>.
    /// </summary>
    public Bootstrapper(IBootstrapperConfig bootstrapperConfig)
    {
        var applicationConfig = bootstrapperConfig.ApplicationConfig;

        var versionLog = new VersionLog(applicationConfig.RootInstallDirectory);

        // Workaround to force creation of dispatcher.
        if (Application.Current == null)
        {
            _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }

        var dispatcher = Application.Current!.Dispatcher;

        var applicationDirectories = new ApplicationDirectories(versionLog, applicationConfig);

        // Do not move this from here.
        var assemblyManager = new AssemblyManager(applicationDirectories, bootstrapperConfig.AssemblyRedirectsSet);

        LoggerService.Initialize(applicationDirectories);

        this.Dispatcher = dispatcher;

        this.AssemblyManager = assemblyManager;

        this.WindowConfig = bootstrapperConfig.WindowConfig;

        this.ApplicationDirectories = applicationDirectories;

        this.VersionLog = versionLog;

        this.BootstrapperConfig = bootstrapperConfig;
    }
}

