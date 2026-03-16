using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCircularArc3d = Autodesk.AutoCAD.Geometry.CircularArc3d;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using CadPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using CadPolyline3d = Autodesk.AutoCAD.DatabaseServices.Polyline3d;
using CadSpline = Autodesk.AutoCAD.DatabaseServices.Spline;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoPolyLineCurve = Rhino.Geometry.PolylineCurve;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Rhino curve types to AutoCAD curve types.
/// </summary>
public static class RhinoCurveExtensions
{
    /// <summary>
    /// Converts a Rhino LineCurve to an AutoCAD Line, applying unit conversion.
    /// </summary>
    /// <param name="line">The Rhino line curve to convert.</param>
    /// <returns>An AutoCAD Line with endpoints scaled to AutoCAD units.</returns>
    public static CadLine ToAutocadLine(this RhinoLineCurve line)
    {
        var startPoint = line.PointAtStart.ToAutocadPoint3d();
        var endPoint = line.PointAtEnd.ToAutocadPoint3d();
        return new CadLine(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a Rhino LineCurve to an AutoCAD LineSegment2d, applying unit conversion.
    /// </summary>
    /// <param name="line">The Rhino line curve to convert.</param>
    /// <returns>An AutoCAD LineSegment2d with endpoints scaled to AutoCAD units.</returns>
    public static LineSegment2d ToAutocadLineSegment2d(this RhinoLineCurve line)
    {
        var startPoint = line.PointAtStart.ToAutocadPoint2d();
        var endPoint = line.PointAtEnd.ToAutocadPoint2d();
        return new LineSegment2d(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a Rhino Arc to an AutoCAD Arc, applying unit conversion.
    /// </summary>
    /// <param name="arc">The Rhino arc to convert.</param>
    /// <returns>An AutoCAD Arc with radius scaled to AutoCAD units.</returns>
    public static CadArc ToAutocadArc(this RhinoArc arc)
    {
        var center = arc.Center.ToAutocadPoint3d();
        var plane = arc.Plane.ToAutocadPlane();
        var normal = plane.Normal;
        var radius = UnitConverter.ToAutoCadLength(arc.Radius);
        var startAngle = arc.StartAngle;
        var endAngle = arc.EndAngle;

        return new CadArc(center, normal, radius, startAngle, endAngle);
    }

    /// <summary>
    /// Converts a Rhino ArcCurve to an AutoCAD Arc, applying unit conversion.
    /// </summary>
    /// <param name="arcCurve">The Rhino arc curve to convert.</param>
    /// <returns>An AutoCAD Arc with radius scaled to AutoCAD units.</returns>
    public static CadArc ToAutocadArc(this RhinoArcCurve arcCurve)
    {
        var midPoint = arcCurve.PointAtNormalizedLength(GeometryConstants.NormalizedMidLength);

        var startPoint = arcCurve.PointAtStart.ToAutocadPoint3d();
        var endPoint = arcCurve.PointAtEnd.ToAutocadPoint3d();
        var pointOnArc = midPoint.ToAutocadPoint3d();

        var circularArc = new CadCircularArc3d(startPoint, pointOnArc, endPoint);

        var cadArc = new CadArc();
        cadArc.SetFromGeCurve(circularArc);

        return cadArc;
    }

    /// <summary>
    /// Converts a Rhino ArcCurve to an AutoCAD CircularArc2d, applying unit conversion.
    /// </summary>
    /// <param name="arcCurve">The Rhino arc curve to convert.</param>
    /// <returns>An AutoCAD CircularArc2d with radius scaled to AutoCAD units.</returns>
    public static CircularArc2d ToAutocadCircularArc2d(this RhinoArcCurve arcCurve)
    {
        var midPoint = arcCurve.PointAtNormalizedLength(GeometryConstants.NormalizedMidLength);

        var startPoint = arcCurve.PointAtStart.ToAutocadPoint2d();
        var endPoint = arcCurve.PointAtEnd.ToAutocadPoint2d();
        var pointOnArc = midPoint.ToAutocadPoint2d();

        return new CircularArc2d(startPoint, pointOnArc, endPoint);
    }

    /// <summary>
    /// Converts a Rhino Circle to an AutoCAD Circle, applying unit conversion.
    /// </summary>
    /// <param name="circle">The Rhino circle to convert.</param>
    /// <returns>An AutoCAD Circle with radius scaled to AutoCAD units.</returns>
    public static CadCircle ToAutocadCircle(this RhinoCircle circle)
    {
        var center = circle.Center.ToAutocadPoint3d();
        var normal = circle.Normal.ToAutocadVector3d();
        var radius = UnitConverter.ToAutoCadLength(circle.Radius);

        return new CadCircle(center, normal, radius);
    }

    /// <summary>
    /// Converts a Rhino Circle to an AutoCAD CircularArc2d (full circle), applying unit conversion.
    /// </summary>
    /// <param name="circle">The Rhino circle to convert.</param>
    /// <returns>An AutoCAD CircularArc2d with radius scaled to AutoCAD units.</returns>
    public static CircularArc2d ToAutocadCircularArc2d(this RhinoCircle circle)
    {
        var center = circle.Center.ToAutocadPoint2d();
        var radius = UnitConverter.ToAutoCadLength(circle.Radius);

        return new CircularArc2d(center, radius);
    }

    /// <summary>
    /// Converts a Rhino Ellipse to an AutoCAD Ellipse, applying unit conversion.
    /// </summary>
    /// <param name="ellipse">The Rhino ellipse to convert.</param>
    /// <returns>An AutoCAD Ellipse with radii scaled to AutoCAD units.</returns>
    public static CadEllipse ToAutocadEllipse(this RhinoEllipse ellipse)
    {
        var plane = ellipse.Plane;
        var centrePoint = ellipse.Center.ToAutocadPoint3d();
        var normal = plane.Normal.ToAutocadVector3d();

        var radius1 = UnitConverter.ToAutoCadLength(ellipse.Radius1);
        var radius2 = UnitConverter.ToAutoCadLength(ellipse.Radius2);

        CadVector3d majorAxisVector;
        double radiusRatio;

        if (radius1 >= radius2)
        {
            // XAxis is the major axis
            majorAxisVector = plane.XAxis.ToAutocadVector3d() * radius1;
            radiusRatio = radius2 / radius1;
        }
        else
        {
            // YAxis is the major axis
            majorAxisVector = plane.YAxis.ToAutocadVector3d() * radius2;
            radiusRatio = radius1 / radius2;
        }

        return new CadEllipse(centrePoint, normal, majorAxisVector, radiusRatio, 0, 2 * Math.PI);
    }

    /// <summary>
    /// Converts a Rhino NurbsCurve to an AutoCAD Spline, applying unit conversion.
    /// </summary>
    /// <param name="nurbsCurve">The Rhino NURBS curve to convert.</param>
    /// <returns>An AutoCAD Spline with control points scaled to AutoCAD units.</returns>
    public static CadSpline ToAutocadSpline(this RhinoNurbsCurve nurbsCurve)
    {
        var cadControlPoints = new CadPoint3dCollection();
        var weightCollection = new DoubleCollection();

        foreach (var rhinoControlPoint in nurbsCurve.Points)
        {
            var cadPoint = rhinoControlPoint.Location.ToAutocadPoint3d();
            var weight = rhinoControlPoint.Weight;
            weightCollection.Add(weight);
            cadControlPoints.Add(cadPoint);
        }

        var rhinoKnotsArray = nurbsCurve.Knots.ToArray();
        var knotCollection = new DoubleCollection(rhinoKnotsArray.Length + 2);

        // Correct Knots from Rhino Nurbs Specification
        knotCollection.Add(rhinoKnotsArray.First());
        knotCollection.AddRange(rhinoKnotsArray);
        knotCollection.Add(rhinoKnotsArray.Last());

        var spline = new CadSpline(nurbsCurve.Degree, nurbsCurve.IsRational,
            nurbsCurve.IsClosed, nurbsCurve.IsPeriodic, cadControlPoints,
            knotCollection, weightCollection, GeometryConstants.FitTolerance, GeometryConstants.FitTolerance);

        spline.UpdateFitData();

        return spline;
    }

    /// <summary>
    /// Converts a Rhino NurbsCurve to an AutoCAD NurbCurve2d, applying unit conversion.
    /// </summary>
    /// <param name="nurbsCurve">The Rhino NURBS curve to convert.</param>
    /// <returns>An AutoCAD NurbCurve2d with control points scaled to AutoCAD units.</returns>
    public static NurbCurve2d ToAutocadNurbCurve2d(this RhinoNurbsCurve nurbsCurve)
    {
        var plane = Rhino.Geometry.Plane.WorldXY;
        var flatCurve = RhinoCurve.ProjectToPlane(nurbsCurve, plane).ToNurbsCurve();

        var fitPoints = new Point2dCollection();
        foreach (var rhinoPoint in flatCurve.Points)
        {
            var cadPoint = rhinoPoint.Location.ToAutocadPoint2d();
            fitPoints.Add(cadPoint);
        }

        return new NurbCurve2d(fitPoints);
    }

    /// <summary>
    /// Converts a Rhino PolylineCurve to an AutoCAD Polyline3d, applying unit conversion.
    /// </summary>
    /// <param name="polyLineCurve">The Rhino polyline curve to convert.</param>
    /// <returns>An AutoCAD Polyline3d with vertices scaled to AutoCAD units.</returns>
    public static CadPolyline3d ToAutocadPolyline3d(this RhinoPolyLineCurve polyLineCurve)
    {
        var pointCount = polyLineCurve.PointCount;
        var pointCollection = new CadPoint3dCollection();

        for (var j = 0; j < pointCount; j++)
        {
            var rhinoPoint = polyLineCurve.Point(j);
            var cadPoint = rhinoPoint.ToAutocadPoint3d();
            pointCollection.Add(cadPoint);
        }

        return new CadPolyline3d(Poly3dType.SimplePoly, pointCollection, polyLineCurve.IsClosed);
    }

    /// <summary>
    /// Converts a Rhino PolylineCurve to an AutoCAD PolylineCurve2d, applying unit conversion.
    /// </summary>
    /// <param name="polyLineCurve">The Rhino polyline curve to convert.</param>
    /// <returns>An AutoCAD PolylineCurve2d with vertices scaled to AutoCAD units.</returns>
    public static PolylineCurve2d ToAutocadPolylineCurve2d(this RhinoPolyLineCurve polyLineCurve)
    {
        var pointCount = polyLineCurve.PointCount;
        var pointCollection = new Point2dCollection();

        for (var j = 0; j < pointCount; j++)
        {
            var rhinoPoint = polyLineCurve.Point(j);
            var cadPoint = rhinoPoint.ToAutocadPoint2d();
            pointCollection.Add(cadPoint);
        }

        return new PolylineCurve2d(pointCollection);
    }

    /// <summary>
    /// Converts a Rhino NurbsCurve to the most appropriate AutoCAD curve type.
    /// Where possible, converts to primitives (Circle, Arc, Ellipse), otherwise to Spline.
    /// </summary>
    /// <param name="nurbsCurve">The Rhino NURBS curve to convert.</param>
    /// <returns>An AutoCAD Curve (Line, Circle, Arc, Ellipse, Polyline, or Spline).</returns>
    public static Curve ToAutocadCurve(this RhinoNurbsCurve nurbsCurve)
    {
        // Degree 1 NURBS curves become polylines
        if (nurbsCurve.Degree == 1)
        {
            var points = nurbsCurve.Points.Select(controlPoint => controlPoint.Location);
            var is2d = true;
            var pointCollection = new Point3dCollection();
            var polyline2d = new CadPolyline();
            var i = 0;

            foreach (var point3d in points)
            {
                if (point3d.Z > GeometryConstants.ZeroTolerance) is2d = false;

                var cadPoint = point3d.ToAutocadPoint3d();
                var cadPoint2d = point3d.ToAutocadPoint2d();

                pointCollection.Add(cadPoint);
                polyline2d.AddVertexAt(i, cadPoint2d, 0, 0, 0);
                i++;
            }

            return is2d ? polyline2d :
                new CadPolyline3d(Poly3dType.SimplePoly, pointCollection, nurbsCurve.IsClosed);
        }

        // Check circles before arcs as Rhino Circles are arcs
        if (nurbsCurve.IsCircle(GeometryConstants.ZeroTolerance) &&
            nurbsCurve.TryGetCircle(out var nurbCircle, GeometryConstants.ZeroTolerance))
        {
            return nurbCircle.ToAutocadCircle();
        }

        if (nurbsCurve.IsArc(GeometryConstants.ZeroTolerance) &&
            nurbsCurve.TryGetArc(out var nurbArc, GeometryConstants.ZeroTolerance))
        {
            var arcCurve = new Rhino.Geometry.ArcCurve(nurbArc);
            return arcCurve.ToAutocadArc();
        }

        if (nurbsCurve.IsEllipse(GeometryConstants.ZeroTolerance) &&
            nurbsCurve.TryGetEllipse(out var ellipse, GeometryConstants.ZeroTolerance))
        {
            return ellipse.ToAutocadEllipse();
        }

        return nurbsCurve.ToAutocadSpline();
    }

    /// <summary>
    /// Converts a Rhino Curve to a list of AutoCAD Curves.
    /// </summary>
    /// <param name="curve">The Rhino curve to convert.</param>
    /// <returns>A list of AutoCAD Curves. Usually contains one curve, but PolyCurves may produce multiple.</returns>
    public static IList<CadCurve> ToAutocadCurves(this RhinoCurve curve)
    {
        switch (curve)
        {
            case RhinoLineCurve line:
                return [line.ToAutocadLine()];

            case RhinoArcCurve arc:
                {
                    if (arc.IsCompleteCircle == false)
                        return [arc.ToAutocadArc()];

                    var circle = new RhinoCircle(arc.Arc);
                    return [circle.ToAutocadCircle()];
                }

            case RhinoNurbsCurve nurbsCurve:
                return [nurbsCurve.ToAutocadCurve()];

            case RhinoPolyLineCurve polyLineCurve:
                return [polyLineCurve.ToAutocadPolyline3d()];

            case RhinoPolyCurve polyCurve:
                return polyCurve.ToAutocadCurveList();

            default:
                return [];
        }
    }

    /// <summary>
    /// Converts a Rhino Curve to a list of AutoCAD 2D Curves.
    /// </summary>
    /// <param name="curve">The Rhino curve to convert.</param>
    /// <returns>A list of AutoCAD 2D Curves.</returns>
    public static IList<Curve2d> ToAutocadCurves2d(this RhinoCurve curve)
    {
        switch (curve)
        {
            case RhinoLineCurve line:
                return [line.ToAutocadLineSegment2d()];

            case RhinoArcCurve arc:
                {
                    if (arc.IsCompleteCircle == false)
                        return [arc.ToAutocadCircularArc2d()];

                    var circle = new RhinoCircle(arc.Arc);
                    return [circle.ToAutocadCircularArc2d()];
                }

            case RhinoNurbsCurve nurbsCurve:
                return [nurbsCurve.ToAutocadNurbCurve2d()];

            case RhinoPolyLineCurve polyLineCurve:
                return [polyLineCurve.ToAutocadPolylineCurve2d()];

            case RhinoPolyCurve polyCurve:
                return polyCurve.ToAutocadCurves2dList();

            default:
                return [];
        }
    }

    /// <summary>
    /// Converts a Rhino Curve to a single AutoCAD Curve.
    /// If the curve would produce multiple segments (like a PolyCurve), it is first converted to a NURBS curve.
    /// </summary>
    /// <param name="curve">The Rhino curve to convert.</param>
    /// <returns>A single AutoCAD Curve.</returns>
    public static CadCurve ToAutocadSingleCurve(this RhinoCurve curve)
    {
        var single = curve is RhinoPolyCurve ? curve.ToNurbsCurve() : curve;

        var listCurves = single.ToAutocadCurves();

        if (listCurves.Count == 1)
            return listCurves[0];

        throw new System.Exception("Cannot convert Rhino curve to single AutoCAD curve.");
    }

    /// <summary>
    /// Converts a Rhino PolyCurve to a list of AutoCAD Curves.
    /// </summary>
    /// <param name="polyCurve">The Rhino polycurve to convert.</param>
    /// <returns>A list of AutoCAD Curves representing all segments.</returns>
    public static IList<CadCurve> ToAutocadCurveList(this RhinoPolyCurve polyCurve)
    {
        var segmentCount = polyCurve.SegmentCount;
        var curves = new List<CadCurve>();

        for (var i = 0; i < segmentCount; i++)
        {
            var segment = polyCurve.SegmentCurve(i);
            var cadCurves = segment.ToAutocadCurves();
            curves.AddRange(cadCurves);
        }

        return curves;
    }

    /// <summary>
    /// Converts a Rhino PolyCurve to a list of AutoCAD 2D Curves.
    /// </summary>
    /// <param name="polyCurve">The Rhino polycurve to convert.</param>
    /// <returns>A list of AutoCAD 2D Curves representing all segments.</returns>
    public static IList<Curve2d> ToAutocadCurves2dList(this RhinoPolyCurve polyCurve)
    {
        var segmentCount = polyCurve.SegmentCount;
        var curves = new List<Curve2d>();

        for (var i = 0; i < segmentCount; i++)
        {
            var segment = polyCurve.SegmentCurve(i);
            var cadCurves = segment.ToAutocadCurves2d();
            curves.AddRange(cadCurves);
        }

        return curves;
    }

    /// <summary>
    /// Converts a Rhino Point to an AutoCAD DBPoint.
    /// </summary>
    /// <param name="point">The Rhino point to convert.</param>
    /// <returns>An AutoCAD DBPoint with coordinates scaled to AutoCAD units.</returns>
    public static CadDBPoint ToAutocadDBPoint(this RhinoPoint point)
    {
        var point3d = point.Location.ToAutocadPoint3d();
        return new CadDBPoint(point3d);
    }
}
