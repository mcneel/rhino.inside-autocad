namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The primary application-wide service all applications depend on.
/// Provides essential services for the entire application and includes
/// methods for managing all other application <see cref="ISatelliteService"/>'s.
/// </summary>
public interface IApplicationServicesCore
{
    /// <summary>
    /// The event raised when the call to <see cref="RegisterSatelliteService"/>
    /// has completed starting all <see cref="ISatelliteService"/>'s.
    /// </summary>
    event EventHandler ServiceRegistrationCompleted;

    /// <summary>
    /// Registers a <see cref="ISatelliteService"/> and calls its StartUp method.
    /// </summary>
    void RegisterSatelliteService(ISatelliteService satelliteService);

    /// <summary>
    /// Restarts all registered <see cref="ISatelliteService"/>'s.
    /// </summary>
    RunResult RestartServices();

    /// <summary>
    /// Shuts down all registered <see cref="ISatelliteService"/>'s and performs
    /// the operations necessary to shut down the software.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Returns the first failed <see cref="ISatelliteService"/> or null
    /// if none failed.
    /// </summary>
    ISatelliteService? GetFailedService();
}