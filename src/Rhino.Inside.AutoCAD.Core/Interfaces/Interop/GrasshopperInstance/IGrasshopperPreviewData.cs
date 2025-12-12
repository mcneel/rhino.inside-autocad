namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a container for preview geometry data, including wires, meshes, and
/// entities.
/// </summary>
public interface IGrasshopperPreviewData
{
    /// <summary>
    /// Gets the collection of wireframe curves representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.Curve> Wires { get; }

    /// <summary>
    /// Gets the collection of meshes representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.Mesh> Meshes { get; }

    /// <summary>
    /// Gets the collection of meshes representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.Point> Points { get; }


    /// <summary>
    /// Gets the collection of text representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.TextEntity> Texts { get; }

    /// <summary>
    /// Gets the collection of dimensions representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.Dimension> Dimensions { get; }

    /// <summary>
    /// Gets the collection of leaders representing the preview geometry.
    /// </summary>
    List<Rhino.Geometry.Leader> Leaders { get; }

    /// <summary>
    /// Retrieves the set of shaded objects from the preview data as a
    /// <see cref="IRhinoConvertibleSet"/>.
    /// </summary>
    IRhinoConvertibleSet GetShadedObjects();

    /// <summary>
    /// Retrieves the set of wireframe objects from the preview data as a
    /// <see cref="IRhinoConvertibleSet"/>.
    /// </summary
    IRhinoConvertibleSet GetWireframeObjects();
}
