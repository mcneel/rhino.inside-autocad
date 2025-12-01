using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IDebugInfo"/>
public class DebugInfo : IDebugInfo
{
    private const string _exceptionBody = CoreMessageConstants.ExceptionBody;

    /// <inheritdoc />
    public string ExceptionTitle { get; }

    /// <inheritdoc />
    public string ExceptionMessage { get; }

    /// <inheritdoc />
    public string ExceptionBody { get; }

    /// <summary>
    /// Constructs a new <see cref="DebugInfo"/> instance with the provided exception,
    /// </summary>
    public DebugInfo(Exception exception, string title, string message)
    {
        this.ExceptionTitle = title;

        this.ExceptionMessage = message;

        this.ExceptionBody = string.Format(_exceptionBody,
            exception.Message,
            exception.StackTrace,
            exception.InnerException != null ? exception.InnerException.Message : string.Empty,
            exception.InnerException != null ? exception.InnerException.StackTrace : string.Empty
        );
    }
}