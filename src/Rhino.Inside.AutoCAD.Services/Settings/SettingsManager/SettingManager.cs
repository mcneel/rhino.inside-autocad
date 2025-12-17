using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ISettingsManager"/>
public class SettingManager : ISettingsManager
{
    private readonly IApplicationDirectories _applicationDirectories;

    /// <inheritdoc />
    public ISettingsCore Core { get; }

    /// <inheritdoc />
    public IUserSettings User { get; }

    /// <summary>
    /// Constructor for the <see cref="SettingManager"/>.
    /// </summary>
    public SettingManager(IApplicationDirectories applicationDirectories)
    {
        _applicationDirectories = applicationDirectories;

        var coreSettingImporter = new SettingCoreImporter();

        var userSettingImporter = new UserSettingImporter();

        this.Core = coreSettingImporter.Import(applicationDirectories);

        this.User = userSettingImporter.Import(applicationDirectories);
    }

    /// <inheritdoc />
    public void SaveUserSettings()
    {
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var userSettingsPath = $"{_applicationDirectories.UserLocal}{CoreConstants.UserSettingsJsonName}";

        var jsonString = JsonSerializer.Serialize((UserSettings)this.User, serializerOptions);

        File.WriteAllText(userSettingsPath, jsonString);
    }
}