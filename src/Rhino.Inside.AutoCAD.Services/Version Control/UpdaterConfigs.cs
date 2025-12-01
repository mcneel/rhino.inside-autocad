using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IUpdaterConfigs"/>
public class UpdaterConfigs : IUpdaterConfigs
{
    /// <inheritdoc />
    public string ManifestFileName { get; }

    /// <inheritdoc />
    public string ReleasesFileName { get; }

    /// <inheritdoc />
    public string AppVersionName { get; }

    /// <summary>
    /// Constructs a new <see cref="UpdaterConfigs"/>.
    /// </summary>
    public UpdaterConfigs(string manifestFileName, string releasesFileName, string appVersionName)
    {
        this.ManifestFileName = manifestFileName;

        this.ReleasesFileName = releasesFileName;

        this.AppVersionName = appVersionName;
    }
}    