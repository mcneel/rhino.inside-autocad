using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ISettingsImporter"/>
public class SettingsImporter : ISettingsImporter
{
    private readonly string _settingCoreJsonName = ApplicationConstants.SettingJsonName;

    /// <summary>
    /// Returns the <see cref="ISettings"/> instance from JSON.
    /// </summary>
    public ISettings Import(IInstallationDirectories installationDirectories)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new InterfaceConverterFactory(typeof(LoadingScreenConstants), typeof(ILoadingScreenConstants)),
                new InterfaceConverterFactory(typeof(Settings), typeof(ISettings))
            }
        };

        var settingsFilePath = $"{installationDirectories.Resources}{_settingCoreJsonName}";

        using var jsonFileStream = File.OpenRead(settingsFilePath);

        var settingsCore = JsonSerializer.Deserialize<ISettings>(jsonFileStream, serializerOptions);

        return settingsCore!;
    }
}