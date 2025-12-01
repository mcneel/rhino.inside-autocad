namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Logger class for golden thread operations and general logging.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Logs a general informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogMessage(string message);

    /// <summary>
    /// Logs a UI interaction event. e.g a button command
    /// </summary>
    /// <param name="commandName">The name of the command to log.</param>
    void LogUIInteragtion(string commandName, string viewModelName);

    /// <summary>
    /// Logs an error with optional extra message and golden thread scope information.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="optionalExtraMessage">An optional extra message to include in the log.</param>
    void LogError(Exception ex, string optionalExtraMessage = "");

    /// <summary>
    /// Shuts down the logger and releases resources.
    /// </summary>
    void Shutdown();
}