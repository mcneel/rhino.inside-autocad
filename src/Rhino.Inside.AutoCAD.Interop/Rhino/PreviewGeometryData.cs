using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperPreviewData"/>
public class GrasshopperPreviewData : IGrasshopperPreviewData
{
    private readonly IRhinoConvertibleFactory _rhinoConvertibleFactory;

    /// <inheritdoc />
    public List<Curve> Wires { get; }

    /// <inheritdoc />
    public List<Mesh> Meshes { get; }

    /// <inheritdoc />
    public List<Point> Points { get; }

    public List<TextEntity> Texts { get; }

    /// <summary>
    /// Constructs a new empty <see cref="IGrasshopperPreviewData"/> instance.
    /// </summary>
    public GrasshopperPreviewData(IRhinoConvertibleFactory rhinoConvertibleFactory)
    {
        _rhinoConvertibleFactory = rhinoConvertibleFactory;
        this.Wires = new List<Curve>();
        this.Meshes = new List<Mesh>();
        this.Points = new List<Point>();
        this.Texts = new List<TextEntity>();
    }

    public IRhinoConvertibleSet GetShadedObjects()
    {
        var shadedSet = new RhinoConvertibleSet();
        foreach (var mesh in this.Meshes)
        {
            if (_rhinoConvertibleFactory.MakeConvertible(mesh, out var result))
            {
                shadedSet.Add(result);
            }
        }
        return shadedSet;
    }

    public IRhinoConvertibleSet GetWireframeObjects()
    {
        var wireFrameSet = new RhinoConvertibleSet();
        foreach (var point3d in this.Points)
        {
            if (_rhinoConvertibleFactory.MakeConvertible(point3d, out var result))
            {
                wireFrameSet.Add(result);
            }
        }

        foreach (var curve in this.Wires)
        {
            if (_rhinoConvertibleFactory.MakeConvertible(curve, out var result))
            {
                wireFrameSet.Add(result);
            }
        }

        foreach (var text in this.Texts)
        {
            if (_rhinoConvertibleFactory.MakeConvertible(text, out var result))
            {
                wireFrameSet.Add(result);
            }
        }

        return wireFrameSet;
    }
}
