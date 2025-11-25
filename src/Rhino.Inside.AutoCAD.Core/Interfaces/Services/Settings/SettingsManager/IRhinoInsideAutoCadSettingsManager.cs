
using Bimorph.Core.Services.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

public interface IRhinoInsideAutoCadSettingsManager :
    ISettingsManager<IRhinoInsideAutoCadApplicationSettings, IRhinoInsideAutoCadUserSettings>
{
    /// <summary>
    /// Saves the current user settings to the JSON file.
    /// </summary>
    void SaveUserSettings();
}