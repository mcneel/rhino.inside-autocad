namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The application interface for Rhino.Inside.AutoCAD applications. In addition, to the
/// <see cref="IBootstrapper"/> and <see cref="IApplicationServicesCore"/>, it is typical
/// for the inheriting interface to specify a <see cref="IFileResourceManager"/> and <see
/// cref="ISettingsManager{TApplicationSettings, TUserSettings}"/>.
/// </summary>
public interface IBimorphApplication
{
    /// <summary>
    /// The bootstrapper for the host application.
    /// </summary>
    IBootstrapper Bootstrapper { get; }

    /// <summary>
    /// The core application services.
    /// </summary>
    IApplicationServicesCore ApplicationServicesCore { get; }

    /// <summary>
    /// The application configuration settings.
    /// </summary>
    IApplicationConfig ApplicationConfig { get; }
}