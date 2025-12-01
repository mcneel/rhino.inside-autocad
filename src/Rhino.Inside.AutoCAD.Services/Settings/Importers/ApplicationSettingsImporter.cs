using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// An importer of application setting for the TemplateName application. <inheritdoc
/// cref="IApplicationSettingsImporter{TApplicationSettings}"/>.
/// </summary>
public class ApplicationSettingsImporter : IApplicationSettingsImporter<IRhinoInsideAutoCadApplicationSettings>
{
    private readonly string _applicationSettingCoreJsonName = CoreConstants.ApplicationSettingCoreJsonName;

    /// <summary>
    /// Returns the <see cref="ISettingsCore"/> instance from JSON.
    /// </summary>
    public IRhinoInsideAutoCadApplicationSettings Import(IApplicationDirectories applicationDirectories)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new InterfaceConverterFactory(typeof(FileNameLibrary), typeof(IFileNameLibrary)),
                new InterfaceConverterFactory(typeof(JsonNameLibrary), typeof(IJsonNameLibrary)),
                new InterfaceConverterFactory(typeof(DataFileName), typeof(IDataFileName)),
                new InterfaceConverterFactory(typeof(ApplicationSettings), typeof(IRhinoInsideAutoCadApplicationSettings))
            }
        };

        var settingsFilePath = $"{applicationDirectories.Resources}{_applicationSettingCoreJsonName}";

        using var jsonFileStream = File.OpenRead(settingsFilePath);

        var settingsCore = JsonSerializer.Deserialize<IRhinoInsideAutoCadApplicationSettings>(jsonFileStream, serializerOptions);

        return settingsCore!;
    }
}