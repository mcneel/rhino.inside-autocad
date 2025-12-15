namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines constants providing messages displayed in the splash screen
/// </summary>
public interface ILoadingScreenConstants
{
    /// <summary>
    /// The copyright notice displayed in the splash screen.
    /// </summary>
    string? Copyright { get; set; }

    /// <summary>
    /// The prefix text added to the <see cref="IVersionLog.CurrentVersion"/>.
    /// </summary>
    string? VersionPrefix { get; set; }

    /// <summary>
    /// The message to display to the user in the splash screen if a
    /// there is an error during startup.
    /// </summary>
    string? FailedServiceMessage { get; set; }
}