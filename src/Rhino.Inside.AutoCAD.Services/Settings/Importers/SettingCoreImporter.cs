using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ISettingCoreImporter"/>
public class SettingCoreImporter : ISettingCoreImporter
{
    private readonly string _settingCoreJsonName = CoreConstants.SettingCoreJsonName;

    /// <summary>
    /// Returns the <see cref="ISettingsCore"/> instance from JSON.
    /// </summary>
    public ISettingsCore Import(IApplicationDirectories applicationDirectories)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new InterfaceConverterFactory(typeof(DeploymentDirectory), typeof(IDeploymentDirectory)),
                new InterfaceConverterFactory(typeof(LoadingScreenConstants), typeof(ILoadingScreenConstants)),
                new InterfaceConverterFactory(typeof(SettingsCore), typeof(ISettingsCore))
            }
        };

        var settingsFilePath = $"{applicationDirectories.Resources}{_settingCoreJsonName}";

        using var jsonFileStream = File.OpenRead(settingsFilePath);

        var settingsCore = JsonSerializer.Deserialize<ISettingsCore>(jsonFileStream, serializerOptions);

        return settingsCore!;
    }
}