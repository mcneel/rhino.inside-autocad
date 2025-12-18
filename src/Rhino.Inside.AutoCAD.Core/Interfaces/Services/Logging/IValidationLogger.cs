namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A logger for storing validation error messages for
/// application start up. It is used to collect validation
/// information before the main logging system is available. 
/// </summary>
public interface IValidationLogger
{
    /// <summary>
    /// True if this <see cref="IValidationLogger"/> contains error
    /// messages, otherwise false.
    /// </summary>
    bool HasValidationErrors { get; }

    /// <summary>
    /// Adds the <paramref name="message"/> to this <see cref="IValidationLogger"/>.
    /// </summary>
    void AddMessage(string message);

    /// <summary>
    /// Gets the most recent message added to this <see cref="IValidationLogger"/>.
    /// </summary>
    string GetMessage();
}