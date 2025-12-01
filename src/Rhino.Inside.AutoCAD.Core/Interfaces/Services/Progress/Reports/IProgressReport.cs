namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which stores details about the progress status of a
/// task.
/// </summary>
public interface IProgressReport
{
    /// <summary>
    /// The <see cref="Percent"/> which can be used as a formatted string.
    /// </summary>
    string TimerText { get; set; }

    /// <summary>
    /// The progress status message.
    /// </summary>
    string StatusMessage { get; set; }

    /// <summary>
    /// True to report a completed process otherwise false.
    /// </summary>
    bool IsComplete { get; set; }
}