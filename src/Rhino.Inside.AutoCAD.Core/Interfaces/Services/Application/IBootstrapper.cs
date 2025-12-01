using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining the base resources for an external application.
/// </summary>
public interface IBootstrapper
{
    /// <summary>
    /// The application dispatcher.
    /// </summary>
    Dispatcher Dispatcher { get; }

    /// <summary>
    /// The <see cref="IAssemblyManager"/>.
    /// </summary>
    IAssemblyManager AssemblyManager { get; }

    /// <summary>
    /// The <see cref="IWindowConfig"/>.
    /// </summary>
    IWindowConfig WindowConfig { get; }

    /// <summary>
    /// Returns the <see cref="IVersionLog"/>.
    /// </summary>
    IVersionLog VersionLog { get; }

    /// <summary>
    /// The <see cref="IApplicationDirectories"/>.
    /// </summary>
    IApplicationDirectories ApplicationDirectories { get; }

    /// <summary>
    /// The <see cref="IBootstrapperConfig"/> which was used to create this
    /// <see cref="IBootstrapper"/>. This will differ for each application.
    /// </summary>
    IBootstrapperConfig BootstrapperConfig { get; }
}