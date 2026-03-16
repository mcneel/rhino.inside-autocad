namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides access to core application settings loaded from configuration files.
/// </summary>
/// <remarks>
/// Contains settings that control application behavior and UI presentation.
/// Settings are typically loaded from JSON configuration files during application startup
/// via <see cref="ISettingsImporter"/>. Accessed through the <see cref="ISettingsManager"/>.
/// </remarks>
/// <seealso cref="ISettingsManager"/>
/// <seealso cref="ISettingsImporter"/>
public interface ISettings
{
    /// <summary>
    /// Gets the <see cref="ILoadingScreenConstants"/> containing splash screen configuration.
    /// </summary>
    /// <remarks>
    /// Includes settings such as display duration, messages, and visual properties
    /// for the loading screen shown during application startup.
    /// </remarks>
    ILoadingScreenConstants LoadingScreenConstants { get; }
}