using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperPreviewData"/>
public class GrasshopperPreviewData : IGrasshopperPreviewData
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public List<Rhino.Geometry.Curve> Wires { get; }

    /// <inheritdoc />
    public List<Rhino.Geometry.Mesh> Meshes { get; }

    /// <summary>
    /// Constructs a new empty <see cref="IGrasshopperPreviewData"/> instance.
    /// </summary>
    public GrasshopperPreviewData()
    {
        this.Wires = new List<Rhino.Geometry.Curve>();
        this.Meshes = new List<Rhino.Geometry.Mesh>();
    }

    /// <inheritdoc />
    public List<IEntity> GetEntities()
    {
        var entities = new List<IEntity>();
        foreach (var curve in this.Wires)
        {
            var cadCurve = _geometryConverter.ToAutoCadSingleCurve(curve);

            var entity = new Entity(cadCurve);

            entities.Add(entity);
        }

        foreach (var mesh in this.Meshes)
        {
            var cadMesh = _geometryConverter.ToAutoCadType(mesh);

            var entity = new Entity(cadMesh);

            entities.Add(entity);
        }

        return entities;
    }
}
