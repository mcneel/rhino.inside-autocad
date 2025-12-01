namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service for showing the progress reporter dialog window
/// for long-running processes that consume the UI thread. Runs
/// in its own thread to remain responsive.
/// </summary>
public interface IProgressReporter : IDisposable
{
    /// <summary>
    /// Shows the progress reporter UI window with a progress bar.
    /// </summary>
    public void Show();
}