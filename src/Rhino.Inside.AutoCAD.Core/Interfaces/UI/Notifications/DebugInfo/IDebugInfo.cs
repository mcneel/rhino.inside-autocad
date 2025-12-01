namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents debug information for exceptions, this can be used to display a
/// dialog to the user which can asset them in their bug reporting.
/// </summary>
public interface IDebugInfo
{
    /// <summary>
    /// The title of the exception dialog
    /// </summary>
    string ExceptionTitle { get; }

    /// <summary>
    /// The message of the error, this is typically a user-friendly instructions for the user
    /// to report the bug.
    /// </summary>
    string ExceptionMessage { get; }

    /// <summary>
    /// The body of the exception, this is a detailed description of the exception which
    /// includes for debugging purposes the exception message, stack trace, inner exception
    /// message and stack trace.
    /// </summary>
    string ExceptionBody { get; }
}
