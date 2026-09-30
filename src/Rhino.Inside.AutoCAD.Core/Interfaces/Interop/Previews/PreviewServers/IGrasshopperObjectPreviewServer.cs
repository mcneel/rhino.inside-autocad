namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Grasshopper objects in AutoCAD using transient entities.
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
    /// The current visibility state of the preview.
    /// </summary>
    GrasshopperPreviewMode PreviewMode { get; }

    /// <summary>
    /// The most entities each underlying preview server draws before the oldest previews are
    /// dropped. Lowering it drops the oldest previews straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxEntityCount { get; set; }

    /// <summary>
    /// Sets the preview mode to the specified <paramref name="previewMode"/>.
    /// </summary>
    void SetMode(GrasshopperPreviewMode previewMode);

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
    /// Redraws every previewed entity with the current <see cref="UnSelectedSettings"/>,
    /// so a change to those settings is seen immediately. The current
    /// <see cref="PreviewMode"/> is preserved.
    /// </summary>
    void RefreshAppearance();
}