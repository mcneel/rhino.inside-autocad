namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Imports core settings from JSON.
/// </summary>
public interface ISettingsImporter
{
    /// <summary>
    /// Import the settings using the provided <see cref="IInstallationDirectories"/>.
    /// </summary>
    ISettings Import(IInstallationDirectories installationDirectories);
}