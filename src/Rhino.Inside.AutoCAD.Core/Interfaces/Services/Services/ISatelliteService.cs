namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which defines an asynchronous satellite service with methods to
/// start the service and shut down the service.
/// </summary>
public interface ISatelliteService
{
    /// <summary>
    /// The event raised once the <see cref="Restart"/> method is called to
    /// restart the service.
    /// </summary>
    event EventHandler? RestartingService;

    /// <summary>
    /// The event raised once restart has completed.
    /// </summary>
    event EventHandler? CompletedRestart;

    /// <summary>
    /// The <see cref="IValidationLogger"/> for this <see cref="ISatelliteService"/>.
    /// </summary>
    IValidationLogger ValidationLogger { get; }

    /// <summary>
    /// The name of this service.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns true if this service is valid and can be started.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// The steps to take when this service is initialized.
    /// </summary>
    RunResult StartUp();

    /// <summary>
    /// The steps to take when this service is shutdown.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Restarts this service.
    /// </summary>
    RunResult Restart();
}