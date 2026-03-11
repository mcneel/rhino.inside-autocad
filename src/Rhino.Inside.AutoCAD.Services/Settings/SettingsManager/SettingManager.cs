using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ISettingsManager"/>
public class SettingManager : ISettingsManager
{
    private readonly IInstallationDirectories _installationDirectories;

    /// <inheritdoc />
    public ISettings Core { get; }

    /// <summary>
    /// Constructor for the <see cref="SettingManager"/>.
    /// </summary>
    public SettingManager(IInstallationDirectories installationDirectories)
    {
        _installationDirectories = installationDirectories;

        var coreSettingImporter = new SettingsImporter();

        this.Core = coreSettingImporter.Import(installationDirectories);

    }
}