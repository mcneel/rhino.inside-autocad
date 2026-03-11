using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides core services and configuration required to initialize Rhino.Inside.AutoCAD.
/// </summary>
/// <remarks>
/// The bootstrapper is created during application startup and provides access to essential
/// services including assembly resolution, installation directories, and version information.
/// It serves as the root dependency container for the application's infrastructure services.
/// </remarks>
/// <seealso cref="IBootstrapperConfig"/>
/// <seealso cref="IAssemblyResolver"/>
public interface IBootstrapper
{
    /// <summary>
    /// Gets the <see cref="IAssemblyResolver"/> that handles assembly version conflicts.
    /// </summary>
    /// <remarks>
    /// The assembly resolver intercepts assembly load requests to redirect conflicting
    /// versions of common .NET libraries to compatible versions bundled with the add-in.
    /// </remarks>
    /// <seealso cref="IAssemblyResolver"/>
    IAssemblyResolver AssemblyResolver { get; }

    /// <summary>
    /// Gets the <see cref="IApplicationVersionHistory"/> for tracking version history and changes.
    /// </summary>
    /// <remarks>
    /// The version log records the installed version and provides access to release notes
    /// and version comparison functionality.
    /// </remarks>
    IApplicationVersionHistory ApplicationVersionHistory { get; }

    /// <summary>
    /// Gets the <see cref="IInstallationDirectories"/> providing paths to add-in resources.
    /// </summary>
    /// <remarks>
    /// Provides access to installation paths including the add-in directory, versioned
    /// assemblies folder, and other resource locations.
    /// </remarks>
    /// <seealso cref="IInstallationDirectories"/>
    IInstallationDirectories InstallationDirectories { get; }

    /// <summary>
    /// Gets the <see cref="IBootstrapperConfig"/> used to initialize this bootstrapper.
    /// </summary>
    /// <remarks>
    /// The configuration varies by host application (AutoCAD, Civil 3D, etc.) and contains
    /// application-specific settings such as product name, version requirements, and paths.
    /// </remarks>
    /// <seealso cref="IBootstrapperConfig"/>
    IBootstrapperConfig BootstrapperConfig { get; }

    /// <summary>
    /// Gets the WPF <see cref="System.Windows.Threading.Dispatcher"/> for the main UI thread.
    /// </summary>
    /// <remarks>
    /// Use this dispatcher to marshal calls to the UI thread when updating WPF controls
    /// or interacting with AutoCAD's UI from background threads.
    /// </remarks>
    Dispatcher Dispatcher { get; }
}