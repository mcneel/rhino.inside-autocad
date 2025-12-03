namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The settings manager, This setting manager is used to import the <see cref="ISettingsCore"/>
/// and the <see cref="IApplicationSettings"/>. The <see cref="ISettingsCore"/> is the location
/// of the core settings which are common for all applications.
/// </summary>
public interface ISettingsManager
{
    /// <summary>
    /// The core settings shared by all applications.
    /// </summary>
    public ISettingsCore Core { get; }

    /// <summary>
    /// The application specific settings.
    /// </summary>
    public IApplicationSettings Application { get; }

    /// <summary>
    /// The user specific settings.
    /// </summary>
    public IUserSettings User { get; }

    /// <summary>
    /// Saves the current user settings to the JSON file.
    /// </summary>
    void SaveUserSettings();
}