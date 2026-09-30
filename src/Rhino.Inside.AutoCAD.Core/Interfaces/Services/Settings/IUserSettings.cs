namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides access to the user-scoped settings which persist between sessions.
/// </summary>
/// <remarks>
/// Unlike <see cref="ISettings"/>, which is read-only and shipped with the installer,
/// these settings are owned by the user, are mutable, and are stored outside the
/// installation directory so they survive an upgrade. Persisted by the
/// <see cref="IUserSettingsStore"/>.
/// </remarks>
/// <seealso cref="IUserSettingsStore"/>
public interface IUserSettings
{
    /// <summary>
    /// The registry version key of the Rhino installation the user last chose to use,
    /// for example "8.0" or "9.0". Null when the user has never made a choice.
    /// </summary>
    /// <remarks>
    /// When <see cref="AlwaysUsePreferredRhinoVersion"/> is true this is the version
    /// Rhino.Inside binds to without prompting. When it is false this is only used to
    /// pre-select a version in the startup dialog.
    /// </remarks>
    /// <seealso cref="AlwaysUsePreferredRhinoVersion"/>
    string? PreferredRhinoVersion { get; set; }

    /// <summary>
    /// True when <see cref="PreferredRhinoVersion"/> should be used without prompting,
    /// otherwise false to ask the user each time AutoCAD starts.
    /// </summary>
    /// <seealso cref="PreferredRhinoVersion"/>
    bool AlwaysUsePreferredRhinoVersion { get; set; }

    /// <summary>
    /// The AutoCAD Color Index (1-255) the previews of Rhino geometry are drawn in while
    /// they are not selected.
    /// </summary>
    /// <seealso cref="SelectedPreviewColorIndex"/>
    int RhinoPreviewColorIndex { get; set; }

    /// <summary>
    /// The AutoCAD Color Index (1-255) the previews of Grasshopper geometry are drawn in
    /// while they are not selected.
    /// </summary>
    /// <seealso cref="SelectedPreviewColorIndex"/>
    int GrasshopperPreviewColorIndex { get; set; }

    /// <summary>
    /// The AutoCAD Color Index (1-255) previews are drawn in while they are selected, shared
    /// by the Rhino and Grasshopper previews.
    /// </summary>
    /// <seealso cref="RhinoPreviewColorIndex"/>
    /// <seealso cref="GrasshopperPreviewColorIndex"/>
    int SelectedPreviewColorIndex { get; set; }

    /// <summary>
    /// The most AutoCAD entities each preview server draws. When a new preview would exceed
    /// it, the oldest previews are dropped, and a single preview larger than it is truncated.
    /// </summary>
    /// <remarks>
    /// Applied per preview server: the Rhino preview, and the Grasshopper shaded and
    /// wireframe previews, each draw up to this many entities.
    /// </remarks>
    int MaxPreviewEntityCount { get; set; }
}
