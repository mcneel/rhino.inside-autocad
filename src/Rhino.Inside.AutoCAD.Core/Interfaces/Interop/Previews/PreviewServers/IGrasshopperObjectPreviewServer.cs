namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Grasshopper objects in AutoCAD using transient entities.
/// </summary>
public interface IGrasshopperObjectPreviewServer
{
    /// <summary>
    /// The settings used to configure the geometry preview.
    /// </summary>
    IGeometryPreviewSettings Settings { get; }

    /// <summary>
    /// The current visibility state of the preview.
    /// </summary>
    GrasshopperPreviewMode PreviewMode { get; }

    /// <summary>
    /// Sets the preview mode to the specified <paramref name="previewMode"/>.
    /// </summary>
    void SetMode(GrasshopperPreviewMode previewMode);

    /// <summary>
    /// Adds the provided <paramref name="grasshopperPreviewData"/> into this <see cref=
    /// "ITransientManager"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IGrasshopperPreviewData grasshopperPreviewData);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "ITransientManager"/>.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);
}