namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Stores a log of all software releases.
/// </summary>
public interface IReleases
{
    /// <summary>
    /// The log of releases. 
    /// </summary>
    List<Version> Log { get; set; }

    /// <summary>
    /// Adds a release to the <see cref="Log"/>.
    /// </summary>
    void AddRelease(Version release);

    /// <summary>
    /// Returns the latest release from the <see cref="Log"/>.
    /// </summary>
    Version GetLatestRelease();
}