using Rhino.Geometry;
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

    /// <inheritdoc />
    public List<Point3d> Points { get; }

    public List<IEntity> GetWireframeEntities()
    {
        var entities = new List<IEntity>();

        foreach (var point in this.Points)
        {
            var point3d = _geometryConverter.ToAutoCadType(point);

            var dbPoint = new Autodesk.AutoCAD.DatabaseServices.DBPoint(point3d);

            var entity = new Entity(dbPoint);

            entities.Add(entity);
        }

        foreach (var curve in this.Wires)
        {
            var cadCurve = _geometryConverter.ToAutoCadSingleCurve(curve);

            var entity = new Entity(cadCurve);

            entities.Add(entity);
        }

        return entities;
    }

    public List<IEntity> GetShadedEntities()
    {
        var entities = new List<IEntity>();

        foreach (var mesh in this.Meshes)
        {
            var cadMesh = _geometryConverter.ToAutoCadType(mesh);

            var entity = new Entity(cadMesh);

            entities.Add(entity);
        }

        return entities;
    }

    /// <summary>
    /// Constructs a new empty <see cref="IGrasshopperPreviewData"/> instance.
    /// </summary>
    public GrasshopperPreviewData()
    {
        this.Wires = new List<Rhino.Geometry.Curve>();
        this.Meshes = new List<Rhino.Geometry.Mesh>();
        this.Points = new List<Rhino.Geometry.Point3d>();
    }
}
