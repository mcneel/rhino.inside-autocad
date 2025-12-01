using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Stores the current and previous version of the client application.
/// </summary>
public class VersionLog : IVersionLog
{
    private Version _noVersion = new();

    /// <summary>
    /// The current version number of the application using semantic versioning.
    /// </summary>
    public Version CurrentVersion { get; }

    /// <summary>
    /// The previous version number of the application using semantic versioning.
    /// </summary>
    public Version PreviousVersion { get; }

    /// <summary>
    /// The application root install directory used to search for the application
    /// versions.
    /// </summary>
    public string RootInstallDirectory { get; }

    /// <summary>
    /// Constructs a new <see cref="VersionLog"/>.
    /// </summary>
    public VersionLog(string rootInstallDirectory)
    {
        var versions = new List<Version>();

        if (Directory.Exists(rootInstallDirectory))
        {
            var directories = Directory.GetDirectories(rootInstallDirectory);

            foreach (var directory in directories)
            {
                var folderName = Path.GetFileName(directory);
                
                Version.TryParse(folderName, out var version);

                if (version is not null)
                    versions.Add(version);
            }

            versions.Sort();
        }

        var versionCount = versions.Count;

        var currentVersion = versions.LastOrDefault();

        var previousVersion = versionCount > 1 ? versions[versionCount - 2] : _noVersion;

        this.CurrentVersion = currentVersion == null ? _noVersion : currentVersion;

        this.PreviousVersion = previousVersion;

        this.RootInstallDirectory = rootInstallDirectory;
    }
}