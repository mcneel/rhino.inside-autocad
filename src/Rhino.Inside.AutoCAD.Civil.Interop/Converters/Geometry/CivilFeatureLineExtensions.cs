using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoInterval = Rhino.Geometry.Interval;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoPolylineCurve = Rhino.Geometry.PolylineCurve;
using RhinoVector3d = Rhino.Geometry.Vector3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Civil 3D Feature Lines to Rhino geometry.
/// </summary>
/// <remarks>
/// A feature line is not an AutoCAD <c>Curve</c>, so its geometry is rebuilt from its
/// points. PI points define the horizontal geometry, with each PI-to-PI segment either a
/// straight line or an arc (given by its bulge). Elevation points sit between PIs and add
/// grade breaks without changing the plan geometry.
/// </remarks>
public static class CivilFeatureLineExtensions
{
    private const double _zeroBulge = 1e-9;

    /// <summary>
    /// Converts a Civil 3D Feature Line to a Rhino curve, applying unit conversion.
    /// </summary>
    /// <remarks>
    /// Straight segments become polylines through every point on them. Arc segments are
    /// exact in plan; where the elevation changes along an arc, the arc's control points
    /// are lifted so the elevation varies smoothly between the known points. If the
    /// geometry cannot be rebuilt, a polyline through all points is returned instead.
    /// </remarks>
    /// <param name="featureLine">The feature line to convert.</param>
    /// <returns>A Rhino curve, or null if the feature line has fewer than two points.</returns>
    public static RhinoCurve? ToRhinoCurve(this FeatureLine featureLine)
    {
        List<CadPoint3d> allPoints;
        List<CadPoint3d> piPoints;

        try
        {
            allPoints = featureLine.GetPoints(FeatureLinePointType.AllPoints).Cast<CadPoint3d>().ToList();
            piPoints = featureLine.GetPoints(FeatureLinePointType.PIPoint).Cast<CadPoint3d>().ToList();
        }
        catch
        {
            return null;
        }

        if (allPoints.Count < 2)
            return null;

        var rhinoPoints = allPoints.Select(p => p.ToRhinoPoint3d()).ToList();

        try
        {
            var curve = BuildCurve(featureLine, allPoints, piPoints, rhinoPoints);
            if (curve != null)
                return curve;
        }
        catch
        {
            // Fall back to the polyline below
        }

        return new RhinoPolylineCurve(rhinoPoints);
    }

    /// <summary>
    /// Builds the curve one PI-to-PI segment at a time.
    /// </summary>
    private static RhinoCurve? BuildCurve(FeatureLine featureLine, List<CadPoint3d> allPoints,
        List<CadPoint3d> piPoints, List<RhinoPoint3d> rhinoPoints)
    {
        var piIndices = MatchPIIndices(allPoints, piPoints);

        if (piIndices.Count < 2)
            return null;

        var polyCurve = new RhinoPolyCurve();

        for (var segment = 0; segment < piIndices.Count - 1; segment++)
        {
            var start = piIndices[segment];
            var end = piIndices[segment + 1];

            var spanPoints = rhinoPoints.GetRange(start, end - start + 1);

            var bulge = GetBulgeOrZero(featureLine, segment);

            var segmentCurve = Math.Abs(bulge) < _zeroBulge
                ? BuildStraightSegment(spanPoints)
                : BuildArcSegment(spanPoints, bulge);

            if (segmentCurve == null)
                return null;

            polyCurve.AppendSegment(segmentCurve);
        }

        polyCurve.RemoveNesting();

        return polyCurve.SegmentCount == 1 ? polyCurve.SegmentCurve(0).DuplicateCurve() : polyCurve;
    }

    /// <summary>
    /// Finds the index in <paramref name="allPoints"/> of each PI point, in order.
    /// </summary>
    private static List<int> MatchPIIndices(List<CadPoint3d> allPoints, List<CadPoint3d> piPoints)
    {
        var indices = new List<int>();
        var piCursor = 0;

        for (var i = 0; i < allPoints.Count && piCursor < piPoints.Count; i++)
        {
            if (allPoints[i].IsEqualTo(piPoints[piCursor]))
            {
                indices.Add(i);
                piCursor++;
            }
        }

        // Every PI must be matched, otherwise the segment-to-bulge mapping is unreliable.
        return piCursor == piPoints.Count ? indices : new List<int>();
    }

    private static double GetBulgeOrZero(FeatureLine featureLine, int segmentIndex)
    {
        try
        {
            return featureLine.GetBulge(segmentIndex);
        }
        catch
        {
            return 0.0;
        }
    }

    /// <summary>
    /// A straight segment in plan: a polyline through the PI and any elevation points.
    /// </summary>
    private static RhinoCurve BuildStraightSegment(List<RhinoPoint3d> spanPoints)
    {
        if (spanPoints.Count == 2)
            return new RhinoLineCurve(spanPoints[0], spanPoints[1]);

        return new RhinoPolylineCurve(spanPoints);
    }

    /// <summary>
    /// An arc segment in plan. The plan arc is rebuilt from the chord and bulge, then each
    /// sub-span between consecutive points gets the elevations of its end points.
    /// </summary>
    /// <remarks>
    /// Uses the AutoCAD polyline bulge convention: bulge = tan(included angle / 4), and a
    /// positive bulge runs counter-clockwise from the segment start to its end.
    /// </remarks>
    private static RhinoCurve? BuildArcSegment(List<RhinoPoint3d> spanPoints, double bulge)
    {
        var start = spanPoints[0];
        var end = spanPoints[spanPoints.Count - 1];

        var startPlan = new RhinoPoint3d(start.X, start.Y, 0.0);
        var endPlan = new RhinoPoint3d(end.X, end.Y, 0.0);

        var chord = endPlan - startPlan;
        var chordLength = chord.Length;

        if (chordLength < Rhino.RhinoMath.ZeroTolerance)
            return null;

        // For a counter-clockwise arc (positive bulge) the arc lies to the right of the chord.
        var rightNormal = new RhinoVector3d(chord.Y, -chord.X, 0.0);
        rightNormal.Unitize();

        var sagitta = bulge * chordLength / 2.0;
        var midPlan = startPlan + chord / 2.0 + rightNormal * sagitta;

        var planArc = new RhinoArcCurve(new RhinoArc(startPlan, midPlan, endPlan));

        if (!planArc.IsValid)
            return null;

        // Plan parameter of every point on the segment.
        var parameters = new List<double>(spanPoints.Count) { planArc.Domain.T0 };

        for (var i = 1; i < spanPoints.Count - 1; i++)
        {
            var point = spanPoints[i];
            planArc.ClosestPoint(new RhinoPoint3d(point.X, point.Y, 0.0), out var t);
            parameters.Add(Math.Max(t, parameters[parameters.Count - 1]));
        }

        parameters.Add(planArc.Domain.T1);

        var subSpans = new RhinoPolyCurve();

        for (var i = 0; i < spanPoints.Count - 1; i++)
        {
            var interval = new RhinoInterval(parameters[i], parameters[i + 1]);

            if (interval.Length <= Rhino.RhinoMath.ZeroTolerance)
                continue;

            var subArc = planArc.Trim(interval);

            if (subArc == null)
                return null;

            var subSpan = LiftToElevations(subArc, spanPoints[i].Z, spanPoints[i + 1].Z);

            if (subSpan == null)
                return null;

            subSpans.AppendSegment(subSpan);
        }

        if (subSpans.SegmentCount == 0)
            return null;

        return subSpans.SegmentCount == 1 ? subSpans.SegmentCurve(0) : subSpans;
    }

    /// <summary>
    /// Moves a plan arc to the given start and end elevations. A level arc stays an exact
    /// arc; a sloping arc becomes a NURBS curve whose control points are raised in
    /// proportion to their distance along the arc, which keeps the plan shape exact.
    /// </summary>
    private static RhinoCurve? LiftToElevations(RhinoCurve planArc, double startZ, double endZ)
    {
        if (Math.Abs(endZ - startZ) <= Rhino.RhinoMath.ZeroTolerance)
        {
            var levelArc = planArc.DuplicateCurve();
            levelArc.Translate(0.0, 0.0, startZ);
            return levelArc;
        }

        var nurbs = planArc.ToNurbsCurve();

        if (nurbs == null)
            return null;

        var totalLength = nurbs.GetLength();

        if (totalLength <= Rhino.RhinoMath.ZeroTolerance)
            return null;

        var domain = nurbs.Domain;

        for (var i = 0; i < nurbs.Points.Count; i++)
        {
            var greville = nurbs.GrevilleParameter(i);

            var lengthToHere = greville <= domain.T0
                ? 0.0
                : nurbs.GetLength(new RhinoInterval(domain.T0, Math.Min(greville, domain.T1)));

            var fraction = lengthToHere / totalLength;
            var z = startZ + (endZ - startZ) * fraction;

            var controlPoint = nurbs.Points[i];
            var location = controlPoint.Location;

            nurbs.Points.SetPoint(i, new RhinoPoint3d(location.X, location.Y, z), controlPoint.Weight);
        }

        return nurbs;
    }
}
