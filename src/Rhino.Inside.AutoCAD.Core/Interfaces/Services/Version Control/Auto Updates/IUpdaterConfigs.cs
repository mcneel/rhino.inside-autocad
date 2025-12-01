namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining the configuration values for the
/// <see cref="ISoftwareUpdater"/>.
/// </summary>
public interface IUpdaterConfigs
{
    /// <summary>
    /// The name of the plug-in manifest XML file used by AutoCAD to
    /// load the application.
    /// </summary>
    string ManifestFileName { get; }

    /// <summary>
    /// The name of the JSON file containing the release log.
    /// </summary>
    public string ReleasesFileName { get; }

    /// <summary>
    /// The name of the app version property located in the root node.
    /// </summary>
    string AppVersionName { get; }
}