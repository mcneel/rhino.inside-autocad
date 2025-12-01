using Serilog.Events;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A log message DTO sent via the Bimorph logging pipe.
/// </summary>
public interface ILogPipeMessage
{
    /// <summary>
    /// The name of the supported application sending the log message.
    /// </summary>
    string SupportApplicationName { get; }

    /// <summary>
    /// The Rendered log message.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The timestamp of the log message.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// The log event level.
    /// </summary>
    LogEventLevel Level { get; }

    /// <summary>
    /// Creates a flattened string representation of the log message.
    /// </summary>
    string ToFlattenedString();
}