using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// An example of a user settings importer, for the TemplateName applications.
/// </summary>
public class UserSettingImporter : IUserSettingsImporter<IRhinoInsideAutoCadUserSettings>
{
    private const string _userSettingsJsonName = CoreConstants.UserSettingsJsonName;
    private const string _userSettingDeserializeError = MessageConstants.UserSettingDeserializeError;

    /// <summary>
    /// Returns the <see cref="IUserSettings"/> instance from JSON. The users setting are
    /// first looked for in the UserLocal directory. If not found, the template from the
    /// Resources directory template is copied to the UserLocal directory and used. If the
    /// deserialization fails the user settings are reset to the template and a second attempt
    /// is made to deserialize. If this fails an exception is thrown.
    /// </summary>
    public IRhinoInsideAutoCadUserSettings Import(IApplicationDirectories applicationDirectories)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                    new InterfaceConverterFactory(typeof(UserSettings), typeof(IRhinoInsideAutoCadUserSettings))
                }
        };

        var localSettingsFilePath = $"{applicationDirectories.UserLocal}{_userSettingsJsonName}";

        if (File.Exists(localSettingsFilePath) == false)
        {
            var templatePath = $"{applicationDirectories.Resources}{_userSettingsJsonName}";

            File.Copy(templatePath, localSettingsFilePath, true);
        }

        if (this.TryDeserialize(localSettingsFilePath, serializerOptions,
                out var rhinoInsideAutoCadUserSettings) == false)
        {
            var templatePath = $"{applicationDirectories.Resources}{_userSettingsJsonName}";

            File.Copy(templatePath, localSettingsFilePath, true);
        }

        if (this.TryDeserialize(localSettingsFilePath, serializerOptions,
                out rhinoInsideAutoCadUserSettings) == false)
        {
            throw new Exception(_userSettingDeserializeError);
        }

        return rhinoInsideAutoCadUserSettings!;
    }

    /// <summary>
    /// Tries to deserialize the user settings from the specified file path.
    /// </summary>
    private bool TryDeserialize(string localSettingsFilePath,
        JsonSerializerOptions serializerOptions, out IRhinoInsideAutoCadUserSettings? RhinoInsideAutoCADUserSettings)
    {
        try
        {
            using var jsonFileStream = File.OpenRead(localSettingsFilePath);

            var userSettings = JsonSerializer.Deserialize<IRhinoInsideAutoCadUserSettings>(jsonFileStream, serializerOptions);

            RhinoInsideAutoCADUserSettings = userSettings!;
            return true;
        }
        catch (Exception e)
        {
            RhinoInsideAutoCADUserSettings = null;
            return false;
        }
    }
}