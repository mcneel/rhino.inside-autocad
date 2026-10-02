using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadIntPtrCollection = Autodesk.AutoCAD.Geometry.IntPtrCollection;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
using CadVector3dCollection = Autodesk.AutoCAD.Geometry.Vector3dCollection;
using RhinoPoint3d = Rhino.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewGeometryBuffer"/>
/// <remarks>
/// The point buffers are held both as the native AutoCAD collections, which
/// <see cref="AddPoint"/> appends to directly, and as the wrappers exposed through the
/// interface, which share those same collections.
/// </remarks>
public class PreviewGeometryBuffer : IPreviewGeometryBuffer
{
    private readonly CadPoint3dCollection _points;
    private readonly CadVector3dCollection _pointNormals;
    private readonly CadIntPtrCollection _pointSubEntityMarkers;
    private bool _disposed;

    /// <inheritdoc/>
    public List<IPoint3dCollection> Polylines { get; } = new List<IPoint3dCollection>();

    /// <inheritdoc/>
    public IPoint3dCollection Points { get; }

    /// <inheritdoc/>
    public IVector3dCollection PointNormals { get; }

    /// <inheritdoc/>
    public IIntPtrCollection PointSubEntityMarkers { get; }

    /// <inheritdoc/>
    public List<IPreviewShell> Shells { get; } = new List<IPreviewShell>();

    /// <inheritdoc/>
    public List<IEntity> FallbackEntities { get; } = new List<IEntity>();

    /// <inheritdoc/>
    public int ItemCount => this.Polylines.Count + _points.Count + this.Shells.Count +
                            this.FallbackEntities.Count;

    /// <summary>
    /// Constructs a new, empty <see cref="PreviewGeometryBuffer"/>.
    /// </summary>
    public PreviewGeometryBuffer()
    {
        var points = new CadPoint3dCollection();
        var pointNormals = new CadVector3dCollection();
        var pointSubEntityMarkers = new CadIntPtrCollection();

        _points = points;
        _pointNormals = pointNormals;
        _pointSubEntityMarkers = pointSubEntityMarkers;

        this.Points = new AutocadPoint3dCollectionWrapper(points);
        this.PointNormals = new AutocadVector3dCollectionWrapper(pointNormals);
        this.PointSubEntityMarkers = new AutocadIntPtrCollectionWrapper(pointSubEntityMarkers);
    }

    /// <inheritdoc/>
    public void AddPoint(RhinoPoint3d point)
    {
        var cadPoint = point.ToAutocadPoint3d();

        _points.Add(cadPoint);

        _pointNormals.Add(CadVector3d.ZAxis);

        _pointSubEntityMarkers.Add(IntPtr.Zero);
    }

    /// <summary>
    /// Disposes the <see cref="FallbackEntities"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        foreach (var entity in this.FallbackEntities)
        {
            var cadEntity = entity.Unwrap();

            if (cadEntity.IsDisposed == false)
            {
                cadEntity.Dispose();
            }
        }

        this.FallbackEntities.Clear();
    }
}
