using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IUserSettings"/>
/// <remarks>
/// A plain serializable type deliberately kept free of the interface-typed properties used
/// by <see cref="Settings"/>: the <see cref="InterfaceConverter{TClass, TInterface}"/> stack is
/// read-only, so a settings type that has to be written back to disk cannot use it.
/// </remarks>
public class UserSettings : IUserSettings
{
    /// <inheritdoc/>
    public string? PreferredRhinoVersion { get; set; }

    /// <inheritdoc/>
    public bool AlwaysUsePreferredRhinoVersion { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before preview colors
    /// were configurable, which has no such key, keeps the colors it had.
    /// </remarks>
    public int RhinoPreviewColorIndex { get; set; } =
        ApplicationConstants.DefaultRhinoPreviewColorIndex;

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before preview colors
    /// were configurable, which has no such key, keeps the colors it had.
    /// </remarks>
    public int GrasshopperPreviewColorIndex { get; set; } =
        ApplicationConstants.DefaultGrasshopperPreviewColorIndex;

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before preview colors
    /// were configurable, which has no such key, keeps the colors it had.
    /// </remarks>
    public int SelectedPreviewColorIndex { get; set; } =
        ApplicationConstants.DefaultSelectedPreviewColorIndex;

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before the preview
    /// limit was configurable, which has no such key, gets the default limit.
    /// </remarks>
    public int MaxPreviewEntityCount { get; set; } =
        ApplicationConstants.DefaultMaxPreviewEntityCount;

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before preview hiding
    /// was configurable, which has no such key, keeps hiding the preview.
    /// </remarks>
    public bool HideRhinoPreviewWhenWindowHidden { get; set; } =
        ApplicationConstants.DefaultHideRhinoPreviewWhenWindowHidden;

    /// <inheritdoc/>
    /// <remarks>
    /// Initialised to the shipped default so a settings file written before preview hiding
    /// was configurable, which has no such key, keeps hiding the preview.
    /// </remarks>
    public bool HideGrasshopperPreviewWhenEditorHidden { get; set; } =
        ApplicationConstants.DefaultHideGrasshopperPreviewWhenEditorHidden;
}
