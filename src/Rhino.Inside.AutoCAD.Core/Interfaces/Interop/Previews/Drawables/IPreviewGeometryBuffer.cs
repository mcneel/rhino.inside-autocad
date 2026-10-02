namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The primitive buffers an <see cref="IPreviewDrawable"/> draws, already converted to
/// AutoCAD units and built once per rebuild.
/// </summary>
/// <remarks>
/// Only <see cref="FallbackEntities"/> hold native resources, so disposing the buffer
/// disposes those entities and nothing else.
/// </remarks>
public interface IPreviewGeometryBuffer : IDisposable
{
    /// <summary>
    /// Gets the polylines to draw, one <see cref="IPoint3dCollection"/> each.
    /// </summary>
    List<IPoint3dCollection> Polylines { get; }

    /// <summary>
    /// Gets the points to draw. Add to it through <see cref="AddPoint"/> so
    /// <see cref="PointNormals"/> and <see cref="PointSubEntityMarkers"/> stay in step.
    /// </summary>
    IPoint3dCollection Points { get; }

    /// <summary>
    /// Gets one normal per entry of <see cref="Points"/>.
    /// </summary>
    /// <remarks>
    /// AutoCAD's polypoint drawing reads its optional arrays per point, so they are filled
    /// to the same length rather than passed empty or null.
    /// </remarks>
    IVector3dCollection PointNormals { get; }

    /// <summary>
    /// Gets one (zero) sub-entity marker per entry of <see cref="Points"/>.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="PointNormals" path="/remarks"/>
    /// </remarks>
    IIntPtrCollection PointSubEntityMarkers { get; }

    /// <summary>
    /// Gets the shells to draw.
    /// </summary>
    List<IPreviewShell> Shells { get; }

    /// <summary>
    /// Gets the non-database-resident AutoCAD entities drawn for geometry which is
    /// impractical to draw by hand, such as dimensions, leaders, hatches and text.
    /// </summary>
    List<IEntity> FallbackEntities { get; }

    /// <summary>
    /// Gets the number of preview items held: each polyline, point, shell and fallback
    /// entity counts as one.
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// Adds a point, converting it to AutoCAD units and keeping <see cref="PointNormals"/>
    /// and <see cref="PointSubEntityMarkers"/> the same length as <see cref="Points"/>.
    /// </summary>
    /// <param name="point">The point to add, in Rhino units.</param>
    void AddPoint(Rhino.Geometry.Point3d point);
}
