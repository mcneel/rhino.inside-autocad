namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Imports core settings from JSON.
/// </summary>
public interface ISettingCoreImporter
{
    /// <summary>
    /// Import the settings using the provided <see cref="IApplicationDirectories"/>.
    /// </summary>
    ISettingsCore Import(IApplicationDirectories applicationDirectories);
}