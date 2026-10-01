namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A single drawable which previews every item of one registered preview object (a
/// Grasshopper component or a Rhino object), registered with AutoCAD as one transient.
/// </summary>
/// <remarks>
/// The drawable draws its geometry itself rather than registering one AutoCAD entity per
/// item, so the transient count stays at the number of preview objects. The register which
/// owns it must hold a strong reference for as long as it is registered as a transient, and
/// dispose it once it has been erased.
/// </remarks>
public interface IPreviewDrawable : IDisposable
{
    /// <summary>
    /// Gets or sets whether the drawable draws with the selected preview settings rather
    /// than the unselected ones.
    /// </summary>
    /// <remarks>
    /// Changing this does not redraw the drawable: the caller updates the transient so
    /// AutoCAD draws it again.
    /// </remarks>
    bool IsSelected { get; set; }

    /// <summary>
    /// Gets the number of preview items the drawable draws, counted against the preview
    /// server's item limit.
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// Re-applies the current preview settings to anything which caches them.
    /// </summary>
    /// <remarks>
    /// The drawn traits are read from the settings on every draw, so only the fallback
    /// entities hold a copy which goes stale. As with <see cref="IsSelected"/>, the caller
    /// updates the transient afterwards so AutoCAD draws it again.
    /// </remarks>
    void RefreshAppearance();
}
