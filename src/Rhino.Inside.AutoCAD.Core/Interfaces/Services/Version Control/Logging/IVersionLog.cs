namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Stores the current and previous version of the client application.
/// </summary>
public interface IVersionLog
{
    /// <summary>
    /// The current version number of the application using semantic versioning.
    /// </summary>
    Version CurrentVersion { get; }

    /// <summary>
    /// The previous version number of the application using semantic versioning.
    /// </summary>
    Version PreviousVersion { get; }

    /// <summary>
    /// The application root install directory used to search for the application
    /// versions.
    /// </summary>
    string RootInstallDirectory { get; }
}