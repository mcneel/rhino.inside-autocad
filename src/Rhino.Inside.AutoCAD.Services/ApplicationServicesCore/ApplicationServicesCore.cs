using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IApplicationServicesCore"/>
public class ApplicationServicesCore : IApplicationServicesCore
{
    private readonly List<ISatelliteService> _satelliteServices = [];
    private readonly ILoggerService _logger = LoggerService.Instance;

    /// <inheritdoc/>
    public event EventHandler? ServiceRegistrationCompleted;

    /// <summary>
    /// Constructs a new <see cref="IApplicationServicesCore" /> and performs the
    /// startup actions to perform when the software launches.
    /// </summary>
    public ApplicationServicesCore()
    {
    }

    /// <inheritdoc/>
    public void RegisterSatelliteService(ISatelliteService satelliteService)
    {
        _satelliteServices.Add(satelliteService);

        satelliteService.StartUp();

        this.OnServiceRegistrationCompleted(EventArgs.Empty);
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        // Shutdown in reverse order so any dependencies on parent services aren't shutdown first.
        for (var i = _satelliteServices.Count - 1; i >= 0; i--)
        {
            var service = _satelliteServices[i];
            try
            {
                service.Shutdown();
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }
        }
    }

    /// <inheritdoc/>
    public RunResult RestartServices()
    {
        try
        {
            foreach (var satelliteService in _satelliteServices)
            {
                satelliteService.Restart();
            }
        }
        catch
        {
            return RunResult.Failed;
        }

        return RunResult.Success;
    }

    /// <inheritdoc/>
    public ISatelliteService GetFailedService()
    {
        return _satelliteServices.First(service => service.ValidationLogger.HasValidationErrors);
    }

    /// <summary>
    /// Event handler raised when the application all <see cref="ISatelliteService"/>
    /// passed into the <see cref="RegisterSatelliteService"/> have completed startup.
    /// </summary>
    private void OnServiceRegistrationCompleted(EventArgs e)
    {
        var handler = ServiceRegistrationCompleted;
        handler?.Invoke(this, e);
    }
}