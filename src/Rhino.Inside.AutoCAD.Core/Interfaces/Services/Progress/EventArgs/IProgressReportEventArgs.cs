namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents event args created when the <see cref="IProgressReporter.Report"/>
/// event is raised. Stores the <see cref="IProgressReport"/> details.
/// </summary>
public interface IProgressReportEventArgs
{
    /// <summary>
    /// The <see cref="IProgressReport"/> of this event.
    /// </summary>
    IProgressReport Report { get; }
}