using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A constants class storing messages displayed in the splash screen
/// </summary>
public class LoadingScreenConstants : ILoadingScreenConstants
{
    /// <summary>
    /// The copyright notice displayed in the splash screen.
    /// </summary>
    public string? Copyright { get; set; }

    /// <summary>
    /// The prefix text added to the <see cref="IVersionLog.CurrentVersion"/> number.
    /// </summary>
    public string? VersionPrefix { get; set; }

    /// <summary>
    /// The message to display to the user in the splash screen if a
    /// <see cref="ISatelliteService"/> fails startup.
    /// </summary>
    public string? FailedServiceMessage { get; set; }
}