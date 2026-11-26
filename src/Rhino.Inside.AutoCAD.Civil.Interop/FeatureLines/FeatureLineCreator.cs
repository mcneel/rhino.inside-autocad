using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using Plane = Rhino.Geometry.Plane;
using Point2d = Autodesk.AutoCAD.Geometry.Point2d;
using Point3d = Rhino.Geometry.Point3d;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Provides methods for creating Civil 3D Feature Lines from Rhino curves.
/// </summary>
/// <remarks>
/// <see cref="FeatureLine.Create(string, ObjectId, ObjectId)"/> only accepts an existing
/// database entity, so the curve is first written as a temporary plan polyline with
/// bulges (keeping arcs as arcs). After the feature line is created, the temporary
/// polyline is erased and each PI gets its elevation from the source curve.
/// </remarks>
public static class FeatureLineCreator
{
    private const double _shortCurveTolerance = GeometryConstants.ShortCurveTolernace;
    private const double _angleTolerance = GeometryConstants.AngleTolernace;
    private const double _vertexTolerance = GeometryConstants.VertexTolerance;

    /// <summary>
    /// A plan vertex of the temporary polyline: its location, the bulge of the segment that
    /// starts at it, and the elevation of the source curve at that point.
    /// </summary>
    private readonly struct PlanVertex
    {
        public Point3d Location { get; }
        public double Bulge { get; }
        public double Elevation { get; }

        public PlanVertex(Point3d location, double bulge, double elevation)
        {
            this.Location = location;
            this.Bulge = bulge;
            this.Elevation = elevation;
        }
    }

    /// <summary>
    /// Creates a Civil 3D Feature Line from a Rhino curve.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <param name="curve">The Rhino curve to create the feature line from.</param>
    /// <param name="name">The feature line name. Must be unique among feature lines.</param>
    /// <param name="siteId">The site ObjectId, or <see cref="ObjectId.Null"/> for a siteless feature line.</param>
    /// <param name="layerId">The layer ObjectId for the feature line.</param>
    /// <param name="styleId">The feature line style ObjectId, or <see cref="ObjectId.Null"/> for the default.</param>
    /// <returns>The created Feature Line, opened for write.</returns>
    public static FeatureLine Create(
        IAutocadTransactionManager transactionManager,
        RhinoCurve curve,
        string name,
        ObjectId siteId,
        ObjectId layerId,
        ObjectId styleId)
    {
        var vertices = GetPlanVertices(curve);

        if (vertices.Count < 2)
            throw new ArgumentException("The curve has no usable plan geometry for a feature line.");

        var transaction = transactionManager.Unwrap();
        var database = transactionManager.AutocadDatabase.Unwrap();

        var modelSpace = (BlockTableRecord)transaction.GetObject(
            SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite);

        var polyline = CreatePlanPolyline(vertices, curve.IsClosed);
        polyline.SetDatabaseDefaults(database);
        polyline.LayerId = layerId;

        var polylineId = modelSpace.AppendEntity(polyline);
        transaction.AddNewlyCreatedDBObject(polyline, true);

        ObjectId featureLineId;

        try
        {
            featureLineId = siteId.IsNull
                ? FeatureLine.Create(name, polylineId)
                : FeatureLine.Create(name, polylineId, siteId);
        }
        finally
        {
            if (!polyline.IsErased)
                polyline.Erase();
        }

        var featureLine = (FeatureLine)transaction.GetObject(featureLineId, OpenMode.ForWrite);

        featureLine.LayerId = layerId;

        if (!styleId.IsNull)
            featureLine.StyleId = styleId;

        ApplyElevations(featureLine, vertices);

        return featureLine;
    }

    /// <summary>
    /// Projects the curve to plan, splits it into lines and arcs, and records the source
    /// curve's elevation at every vertex.
    /// </summary>
    private static List<PlanVertex> GetPlanVertices(RhinoCurve curve)
    {
        var vertices = new List<PlanVertex>();

        // Projection is a transform, so the projected curve keeps the source parameterization.
        var projected = RhinoCurve.ProjectToPlane(curve, Plane.WorldXY);

        if (projected == null)
            return vertices;

        var segments = new List<RhinoCurve>();
        AddPlanSegments(projected, segments);

        foreach (var segment in segments)
        {
            var start = segment.PointAtStart;
            vertices.Add(new PlanVertex(start, GetBulge(segment), ElevationAt(curve, projected, start)));
        }

        if (!curve.IsClosed && segments.Count > 0)
        {
            var end = segments[segments.Count - 1].PointAtEnd;
            vertices.Add(new PlanVertex(end, 0.0, ElevationAt(curve, projected, end)));
        }

        return vertices;
    }

    /// <summary>
    /// Breaks a plan curve into line and arc segments. Lines, polylines and arcs are kept
    /// exactly (every polyline vertex becomes a PI), polycurves are handled segment by
    /// segment, closed free-form curves such as ellipses are opened into two halves, and
    /// anything else is fitted with arcs and lines.
    /// </summary>
    private static void AddPlanSegments(RhinoCurve curve, List<RhinoCurve> segments)
    {
        if (TryAddExactSegments(curve, segments))
            return;

        if (curve is RhinoPolyCurve polyCurve)
        {
            for (var i = 0; i < polyCurve.SegmentCount; i++)
                AddPlanSegments(polyCurve.SegmentCurve(i), segments);

            return;
        }

        if (curve.IsClosed)
        {
            // A closed curve has no start or end to anchor the fit, so open it into two
            // halves (each an elliptical or free-form arc) and fit those instead.
            var domain = curve.Domain;

            if (!curve.NormalizedLengthParameter(0.5, out var mid))
                mid = domain.Mid;

            var first = curve.Trim(domain.T0, mid);
            var second = curve.Trim(mid, domain.T1);

            if (first != null && second != null && !first.IsClosed && !second.IsClosed)
            {
                AddPlanSegments(first, segments);
                AddPlanSegments(second, segments);
                return;
            }
        }

        var arcsAndLines = curve.ToArcsAndLines(_vertexTolerance, _angleTolerance * 10.0,
            _shortCurveTolerance, 0.0);

        if (arcsAndLines == null)
        {
            AddApproximateSegments(curve, segments);
            return;
        }

        foreach (var segment in arcsAndLines.Explode())
        {
            if (!TryAddExactSegments(segment, segments))
                AddApproximateSegments(segment, segments);
        }
    }

    /// <summary>
    /// Adds a curve that is already a line, polyline or arc, without approximation.
    /// </summary>
    private static bool TryAddExactSegments(RhinoCurve curve, List<RhinoCurve> segments)
    {
        switch (curve)
        {
            case RhinoLineCurve line:
                if (line.GetLength() > _shortCurveTolerance)
                    segments.Add(line);
                return true;

            case RhinoArcCurve arcCurve:
                AddArcSegments(arcCurve.Arc, segments);
                return true;
        }

        if (curve.TryGetPolyline(out var polyline))
        {
            foreach (var line in polyline.GetSegments())
            {
                if (line.Length > _shortCurveTolerance)
                    segments.Add(new RhinoLineCurve(line));
            }

            return true;
        }

        if (curve is not RhinoPolyCurve && curve.TryGetArc(out var arc, _vertexTolerance))
        {
            AddArcSegments(arc, segments);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds an arc, splitting it into equal pieces of at most 180 degrees. A polyline bulge
    /// cannot describe a full circle, and arcs past a semicircle are split so the bulge stays
    /// at or below one.
    /// </summary>
    private static void AddArcSegments(RhinoArc arc, List<RhinoCurve> segments)
    {
        var arcCurve = new RhinoArcCurve(arc);
        var pieces = (int)Math.Ceiling(arc.Angle / Math.PI - _angleTolerance);

        if (pieces <= 1)
        {
            segments.Add(arcCurve);
            return;
        }

        var domain = arcCurve.Domain;

        for (var i = 0; i < pieces; i++)
        {
            var piece = arcCurve.Trim(domain.ParameterAt((double)i / pieces),
                domain.ParameterAt((double)(i + 1) / pieces));

            if (piece != null)
                segments.Add(piece);
        }
    }

    /// <summary>
    /// Last resort for a curve that cannot be fitted with arcs: a polyline approximation.
    /// </summary>
    private static void AddApproximateSegments(RhinoCurve curve, List<RhinoCurve> segments)
    {
        var approximation = curve.ToPolyline(_vertexTolerance, _angleTolerance * 10.0,
            _shortCurveTolerance, 0.0);

        if (approximation != null)
            TryAddExactSegments(approximation, segments);
    }

    /// <summary>
    /// The bulge of a plan segment: zero for a line, tan(angle / 4) for an arc, positive when
    /// the arc runs counter-clockwise from its start to its end.
    /// </summary>
    private static double GetBulge(RhinoCurve segment)
    {
        if (!segment.TryGetArc(out var arc))
            return 0.0;

        var bulge = Math.Tan(arc.Angle / 4.0);

        return arc.Plane.Normal.Z >= 0.0 ? bulge : -bulge;
    }

    /// <summary>
    /// Finds the elevation of the source curve above a plan point.
    /// </summary>
    private static double ElevationAt(RhinoCurve source, RhinoCurve projected, Point3d planPoint)
    {
        return projected.ClosestPoint(planPoint, out var t)
            ? source.PointAt(t).Z
            : planPoint.Z;
    }

    /// <summary>
    /// Builds the temporary lightweight polyline at elevation zero, applying unit conversion.
    /// </summary>
    private static CadPolyline CreatePlanPolyline(List<PlanVertex> vertices, bool closed)
    {
        var polyline = new CadPolyline();

        for (var i = 0; i < vertices.Count; i++)
        {
            var vertex = vertices[i];
            polyline.AddVertexAt(i, vertex.Location.ToAutocadPoint2d(), vertex.Bulge, 0, 0);
        }

        polyline.Elevation = 0.0;
        polyline.Closed = closed;

        return polyline;
    }

    /// <summary>
    /// Sets the elevation of each feature line point from the source curve. Points are
    /// matched to the plan vertices by plan position rather than by index, so a vertex that
    /// Civil 3D merges or drops does not shift the elevations of the points after it. A
    /// closed feature line that repeats its start point matches the first vertex again.
    /// </summary>
    private static void ApplyElevations(FeatureLine featureLine, List<PlanVertex> vertices)
    {
        var planVertices = vertices
            .Select(v => (Location: v.Location.ToAutocadPoint2d(), Elevation: UnitConverter.ToAutoCadLength(v.Elevation)))
            .ToList();

        var points = featureLine.GetPoints(FeatureLinePointType.AllPoints);
        var tolerance = Tolerance.Global.EqualPoint;

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var planPoint = new Point2d(point.X, point.Y);

            var match = planVertices.FirstOrDefault(v => v.Location.GetDistanceTo(planPoint) <= tolerance);

            if (match.Location.GetDistanceTo(planPoint) <= tolerance)
                featureLine.SetPointElevation(i, match.Elevation);
        }
    }
}
