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
    /// Converts the stored preview geometry into a list of AutoCAD entities.
    /// </summary>
    /// <returns>A list of entities representing the preview geometry.</returns>
    List<IEntity> GetEntities();
}
