namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Logger for storing error messages during application start up. It is used to collect errors
/// This logger is used to collect error messages during application start up before the main
/// logging system is available. It allows for adding error messages and retrieving the most
/// recent error message, as well as checking if any errors have been logged.
/// </summary>
public interface IStartUpLogger
{
    /// <summary>
    /// Boolean property indicating whether this <see cref="IStartUpLogger"/> contains any
    /// error messages.
    /// </summary>
    bool HasError { get; }

    /// <summary>
    /// Adds the specified error message to this <see cref="IStartUpLogger"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddError(string message);

    /// <summary>
    /// Gets the last error message added to this <see cref="IStartUpLogger"/>. If no error
    /// messages have been added, it returns an empty string.
    /// </summary>
    /// <returns></returns>
    string GetLastErrorMessage();
}