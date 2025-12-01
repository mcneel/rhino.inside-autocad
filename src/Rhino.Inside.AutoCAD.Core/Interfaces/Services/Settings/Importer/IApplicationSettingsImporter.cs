namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Imports application settings from JSON.
/// </summary>
public interface IApplicationSettingsImporter<out TApplicationSettings>
    where TApplicationSettings : IApplicationSettings
{
    /// <summary>
    /// Returns the <typeparamref name="TApplicationSettings"/> instance from JSON for a
    /// specific application.
    /// </summary>
    public TApplicationSettings Import(IApplicationDirectories applicationDirectories);
}