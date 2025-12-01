namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface used to define the main entry point into the application.
/// </summary>
public interface IApplicationMain : IDisposable
{
    /// <summary>
    /// Event raised when the application shutdown starts.
    /// </summary>
    event EventHandler ShutdownStarted;

    /// <summary>
    /// Runs the mainline application function asynchronously.
    /// </summary>
    RunResult Run();

    /// <summary>
    /// Shuts down the application.
    /// </summary>
    void Shutdown();
}