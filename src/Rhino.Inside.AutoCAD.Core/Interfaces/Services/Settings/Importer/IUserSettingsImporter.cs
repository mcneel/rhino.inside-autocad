namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Imports user settings from JSON.
/// </summary>
public interface IUserSettingsImporter<out TUserSettings>
    where TUserSettings : IUserSettings
{
    /// <summary>
    /// Returns the <typeparamref name="TUserSettings"/> instance from JSON for a specific application.
    /// </summary>
    public TUserSettings Import(IApplicationDirectories applicationDirectories);
}