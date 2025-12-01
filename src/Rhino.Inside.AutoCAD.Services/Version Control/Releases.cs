using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Stores a log of all software releases.
/// </summary>
public class Releases : IReleases
{
    /// <summary>
    /// The log of releases. 
    /// </summary>
    public List<Version> Log { get; set; }

    /// <summary>
    /// Constructs a new <see cref="Releases"/>.
    /// </summary>
    public Releases()
    {
        this.Log = new List<Version>();
    }

    /// <summary>
    /// Adds a release to the <see cref="Log"/>.
    /// </summary>
    public void AddRelease(Version release)
    {
        if (Log.Contains(release))
            return;

        this.Log.Add(release);
    }

    /// <summary>
    /// Returns the latest release from the <see cref="Log"/>.
    /// </summary>
    public Version GetLatestRelease() 
    {
        return this.Log.Count == 0 ? new Version() : this.Log.Last();
    }
}