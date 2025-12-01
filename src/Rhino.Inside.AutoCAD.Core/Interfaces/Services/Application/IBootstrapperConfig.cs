namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A configuration for the <see cref="IBootstrapper"/> specific to the host application.
/// This config is consumed by the Bootstrapper and models the differences in host
/// applications.
/// </summary>
public interface IBootstrapperConfig
{
    /// <summary>
    /// The configuration for the application window.
    /// </summary>
    IWindowConfig WindowConfig { get; }

    /// <summary>
    /// The set of assembly redirects required by the host application.
    /// </summary>
    IAssemblyRedirectsSet AssemblyRedirectsSet { get; }

    /// <summary>
    /// The application configuration settings.
    /// </summary>
    IApplicationConfig ApplicationConfig { get; }
}