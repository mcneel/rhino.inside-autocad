using Rhino.Inside.AutoCAD.Core.Interfaces;
using Serilog.Events;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ILogPipeMessage"/>
public class LogPipeMessage : ILogPipeMessage
{
    private const string _logPipeMessageFlattenedFormat =
        CoreMessageConstants.LogPipMessageFlattenedFormat;

    /// <inheritdoc />
    public string SupportApplicationName { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc />
    public LogEventLevel Level { get; set; }

    /// <inheritdoc />
    public string ToFlattenedString()
    {
        return string.Format(_logPipeMessageFlattenedFormat,
            this.SupportApplicationName,
            this.Timestamp.ToString("o"),
            this.Level.ToString(),
            this.Message);
    }
}