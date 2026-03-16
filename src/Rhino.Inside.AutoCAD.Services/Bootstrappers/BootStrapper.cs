using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IBootstrapper"/>
/// <remarks>
/// Initializes core infrastructure services during application startup including assembly
/// resolution, logging, and version tracking. This class must be instantiated early in
/// the application lifecycle before any dependent assemblies are loaded.
/// </remarks>
/// <seealso cref="IBootstrapper"/>
/// <seealso cref="IBootstrapperConfig"/>
/// <seealso cref="AssemblyResolver"/>
public class Bootstrapper : IBootstrapper
{
    /// <inheritdoc/>
    public Dispatcher Dispatcher { get; }

    /// <inheritdoc/>
    public IAssemblyResolver AssemblyResolver { get; }

    /// <inheritdoc/>
    public IApplicationVersionHistory ApplicationVersionHistory { get; }

    /// <inheritdoc/>
    public IInstallationDirectories InstallationDirectories { get; }

    /// <inheritdoc/>
    public IBootstrapperConfig BootstrapperConfig { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="bootstrapperConfig">
    /// The host application configuration containing product-specific settings.
    /// </param>
    /// <remarks>
    /// The constructor performs initialization in a specific order:
    /// <list type="number">
    ///     <item>
    ///         <description>
    ///             Creates <see cref="ApplicationVersionHistory"/> from the root install directory.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Ensures a WPF <see cref="Application"/> instance exists to obtain the
    ///             <see cref="Dispatcher"/>. A new application is created with
    ///             <see cref="ShutdownMode.OnExplicitShutdown"/> if none exists.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Creates <see cref="InstallationDirectories"/> for resource path resolution.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Creates <see cref="AssemblyResolver"/> to handle assembly version conflicts.
    ///             This must occur before any dependent assemblies are loaded.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Initializes <see cref="LoggerService"/> with the application directories.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public Bootstrapper(IBootstrapperConfig bootstrapperConfig)
    {
        var applicationConfig = bootstrapperConfig.ApplicationConfig;

        var versionHistory = new ApplicationVersionHistory(applicationConfig.RootInstallDirectory);

        if (Application.Current == null)
        {
            _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }

        var dispatcher = Application.Current!.Dispatcher;

        var applicationDirectories = new InstallationDirectories(versionHistory, applicationConfig);

        var assemblyManager = new AssemblyResolver(applicationDirectories, bootstrapperConfig.AssemblyRedirectsSet);

        LoggerService.Initialize(applicationDirectories);

        this.Dispatcher = dispatcher;

        this.AssemblyResolver = assemblyManager;

        this.InstallationDirectories = applicationDirectories;

        this.ApplicationVersionHistory = versionHistory;

        this.BootstrapperConfig = bootstrapperConfig;
    }
}
