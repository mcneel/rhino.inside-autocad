using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// The <see cref="ISatelliteService"/> base class.
/// </summary>
public abstract class SatelliteServiceBase : ISatelliteService
{
    /// <inheritdoc/>
    public event EventHandler? RestartingService;

    /// <inheritdoc/>
    public event EventHandler? CompletedRestart;

    /// <inheritdoc/>
    public IValidationLogger ValidationLogger { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool IsValid => this.ValidationLogger.HasValidationErrors == false;

    /// <summary>
    /// Constructs a new <see cref="SatelliteServiceBase"/>.
    /// </summary>
    protected SatelliteServiceBase(string name)
    {
        this.ValidationLogger = new ValidationLogger();
        this.Name = name;
    }

    /// <summary>
    /// Performs any tasks required to restart the service. Can be overridden by
    /// any <see cref="ISatelliteService"/> class that inherits from this class.
    /// </summary>
    protected virtual void RestartTasks()
    {

    }

    /// <inheritdoc/>
    public abstract RunResult StartUp();

    /// <inheritdoc/>
    public virtual void Shutdown()
    {

    }

    /// <summary>
    /// Restarts the service.
    /// </summary>
    public RunResult Restart()
    {
        this.OnRestartingService(EventArgs.Empty);

        this.RestartTasks();

        var result = this.StartUp();

        this.OnCompletedRestart(EventArgs.Empty);

        return result;
    }

    /// <summary>
    /// Event handler which raises the <see cref="RestartingService"/> event.
    /// </summary>
    protected void OnRestartingService(EventArgs e)
    {
        var handler = RestartingService;
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// Event handler which raises the <see cref="RestartingService"/> event.
    /// </summary>
    protected void OnCompletedRestart(EventArgs e)
    {
        var handler = CompletedRestart;
        handler?.Invoke(this, e);
    }
}