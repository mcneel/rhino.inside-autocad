namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Grasshopper objects in AutoCAD using transients, one per
/// previewed object and preview server.
/// </summary>
public interface IGrasshopperObjectPreviewServer
{
    /// <summary>
    /// The settings used to configure the geometry preview in the non-selected state.
    /// </summary>
    IGeometryPreviewSettings UnSelectedSettings { get; }

    /// <summary>
    /// The settings used to configure the geometry preview in the selected state.
    /// </summary>
    IGeometryPreviewSettings SelectedSettings { get; }

    /// <summary>
    /// The preview mode the user has chosen.
    /// </summary>
    /// <remarks>
    /// The preview is drawn in this mode while <see cref="IsSuppressed"/> is false, so this
    /// keeps reporting the user's choice while the preview is suppressed.
    /// </remarks>
    GrasshopperPreviewMode PreviewMode { get; }

    /// <summary>
    /// True while the preview is hidden regardless of <see cref="PreviewMode"/>, for
    /// example while the Grasshopper editor is minimised.
    /// </summary>
    bool IsSuppressed { get; }

    /// <summary>
    /// The most preview items each underlying preview server draws before the oldest previews are
    /// dropped. Lowering it drops the oldest previews straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxItemCount { get; set; }

    /// <summary>
    /// Sets the preview mode to the specified <paramref name="previewMode"/>.
    /// </summary>
    void SetMode(GrasshopperPreviewMode previewMode);

    /// <summary>
    /// Sets <see cref="IsSuppressed"/>, hiding the preview while it is true and drawing it
    /// in <see cref="PreviewMode"/> again afterwards. <see cref="PreviewMode"/> is left as it
    /// is.
    /// </summary>
    void SetSuppressed(bool suppressed);

    /// <summary>
    /// Removes every preview from the shaded and wireframe servers, disposing its drawables.
    /// <see cref="PreviewMode"/> and <see cref="IsSuppressed"/> are left as they are, so
    /// previews added afterwards are drawn as before.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Adds the provided <paramref name="grasshopperPreviewData"/> into this <see cref=
    /// "IGrasshopperObjectPreviewServer"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IGrasshopperPreviewData grasshopperPreviewData);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "IGrasshopperObjectPreviewServer"/>.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Sets the selection state of the preview registered under <paramref name="rhinoObjectId"/>
    /// in both the shaded and wireframe servers and redraws it, without re-extracting or
    /// rebuilding its geometry.
    /// </summary>
    /// <returns>
    /// True when either server has a preview registered under <paramref name="rhinoObjectId"/>,
    /// otherwise false, in which case the caller has to add the object to preview it.
    /// </returns>
    bool SetSelected(Guid rhinoObjectId, bool selected);

    /// <summary>
    /// Redraws every preview with the current <see cref="UnSelectedSettings"/> and
    /// <see cref="SelectedSettings"/>, so a change to those settings is seen immediately.
    /// Nothing is rebuilt, each preview keeps its selection state and the current
    /// <see cref="PreviewMode"/> is preserved.
    /// </summary>
    void RefreshAppearance();
}