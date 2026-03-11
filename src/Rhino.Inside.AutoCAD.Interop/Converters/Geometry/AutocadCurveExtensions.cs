using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoInterval = Rhino.Geometry.Interval;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoVector3d = Rhino.Geometry.Vector3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting AutoCAD curve types to Rhino curve types.
/// </summary>
public static class AutocadCurveExtensions
{
    /// <summary>
    /// Converts an AutoCAD Line to a Rhino LineCurve, applying unit conversion.
    /// </summary>
    /// <param name="line">The AutoCAD line to convert.</param>
    /// <returns>A Rhino LineCurve with endpoints scaled to Rhino units.</returns>
    public static RhinoLineCurve ToRhinoLineCurve(this CadLine line)
    {
        var startPoint = line.StartPoint.ToRhinoPoint3d();
        var endPoint = line.EndPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD Arc to a Rhino Arc, applying unit conversion.
    /// </summary>
    /// <param name="arc">The AutoCAD arc to convert.</param>
    /// <returns>A Rhino Arc with radius scaled to Rhino units.</returns>
    public static RhinoArc ToRhinoArc(this CadArc arc)
    {
        var plane = arc.GetPlane().ToRhinoPlane();
        var radius = UnitConverter.ToRhinoLength(arc.Radius);
        var sweep = arc.TotalAngle;

        var rhinoArc = new RhinoArc(plane, radius, sweep)
        {
            StartAngle = arc.StartAngle,
            Angle = sweep
        };

        return rhinoArc;
    }

    /// <summary>
    /// Converts an AutoCAD Circle to a Rhino Circle, applying unit conversion.
    /// </summary>
    /// <param name="circle">The AutoCAD circle to convert.</param>
    /// <returns>A Rhino Circle with radius scaled to Rhino units.</returns>
    public static RhinoCircle ToRhinoCircle(this CadCircle circle)
    {
        var origin = circle.Center.ToRhinoPoint3d();
        var radius = UnitConverter.ToRhinoLength(circle.Radius);
        return new RhinoCircle(origin, radius);
    }

    /// <summary>
    /// Converts an AutoCAD Ellipse to a Rhino Ellipse, applying unit conversion.
    /// </summary>
    /// <param name="ellipse">The AutoCAD ellipse to convert.</param>
    /// <returns>A Rhino Ellipse with radii scaled to Rhino units.</returns>
    public static RhinoEllipse ToRhinoEllipse(this CadEllipse ellipse)
    {
        var centrePoint = ellipse.Center.ToRhinoPoint3d();
        var majorAxis = ellipse.MajorAxis.ToRhinoVector3d();
        var minorAxis = ellipse.MinorAxis.ToRhinoVector3d();

        var rhinoPlane = new RhinoPlane(centrePoint, majorAxis, minorAxis);

        var radius1 = UnitConverter.ToRhinoLength(ellipse.MajorRadius);
        var radius2 = UnitConverter.ToRhinoLength(ellipse.MinorRadius);

        return new RhinoEllipse(rhinoPlane, radius1, radius2);
    }

    /// <summary>
    /// Converts an AutoCAD Spline to a Rhino NurbsCurve, applying unit conversion.
    /// </summary>
    /// <param name="spline">The AutoCAD spline to convert.</param>
    /// <returns>A Rhino NurbsCurve with control points scaled to Rhino units.</returns>
    public static RhinoNurbsCurve ToRhinoNurbsCurve(this Spline spline)
    {
        var nurbsData = spline.NurbsData;
        var point3dCollection = nurbsData.GetControlPoints();
        var weights = nurbsData.GetWeights();
        var weightsCount = weights.Count;

        var nurbsDataKnots = nurbsData.GetKnots().ToArray().ToList();
        var knots = GetKnots(nurbsDataKnots, nurbsData.Degree, point3dCollection.Count);

        var rhinoNurbsCurve = new RhinoNurbsCurve(spline.Degree, point3dCollection.Count);

        for (var index = 0; index < point3dCollection.Count; index++)
        {
            var point3d = point3dCollection[index].ToRhinoPoint3d();
            var weight = index < weightsCount ? weights[index] : 1.0;
            rhinoNurbsCurve.Points.SetPoint(index, point3d, weight);
        }

        var knotCount = knots.Count;
        var rhinoKnotCollection = rhinoNurbsCurve.Knots;

        for (var d = 0; d < knotCount; d++)
        {
            rhinoKnotCollection[d] = knots[d];
        }

        var rhinoInterval = new RhinoInterval(spline.StartParam, spline.EndParam);
        var trimmedNurbs = rhinoNurbsCurve.Trim(rhinoInterval);

        return trimmedNurbs as RhinoNurbsCurve ?? rhinoNurbsCurve;
    }

    /// <summary>
    /// Converts an AutoCAD LineSegment2d to a Rhino LineCurve, applying unit conversion.
    /// </summary>
    /// <param name="lineSegment2d">The AutoCAD 2D line segment to convert.</param>
    /// <returns>A Rhino LineCurve with endpoints scaled to Rhino units.</returns>
    public static RhinoLineCurve ToRhinoLineCurve(this LineSegment2d lineSegment2d)
    {
        var startPoint = lineSegment2d.StartPoint.ToRhinoPoint2d();
        var endPoint = lineSegment2d.EndPoint.ToRhinoPoint2d();
        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD Line2d to a Rhino LineCurve, applying unit conversion.
    /// </summary>
    /// <param name="line2d">The AutoCAD 2D line to convert.</param>
    /// <returns>A Rhino LineCurve. If the line is infinite, creates a unit-length line.</returns>
    /// <remarks>
    /// <see cref="Line2d"/> are infinite which isn't supported in Rhino. The
    /// <see cref="Line2d.PointOnLine"/> is used as the start point and the
    /// endpoint is the unit translation of its direction.
    /// </remarks>
    public static RhinoLineCurve ToRhinoLineCurve(this Line2d line2d)
    {
        var hasStartPoint = line2d.HasStartPoint;
        var startPoint = hasStartPoint ? line2d.StartPoint.ToRhinoPoint2d() : line2d.PointOnLine.ToRhinoPoint2d();

        var hasEndPoint = line2d.HasEndPoint;
        RhinoPoint2d endPoint;

        if (hasEndPoint)
        {
            endPoint = line2d.EndPoint.ToRhinoPoint2d();
        }
        else
        {
            var vector3d = line2d.Direction.ToRhinoVector3d();
            var translation = Rhino.Geometry.Transform.Translation(vector3d);
            endPoint = new RhinoPoint2d(startPoint);
            endPoint.Transform(translation);
        }

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD CircularArc2d to a Rhino Arc, applying unit conversion.
    /// </summary>
    /// <param name="circularArc2d">The AutoCAD 2D circular arc to convert.</param>
    /// <returns>A Rhino Arc with endpoints scaled to Rhino units.</returns>
    public static RhinoArc ToRhinoArc(this CircularArc2d circularArc2d)
    {
        var startPoint = circularArc2d.StartPoint.ToRhinoPoint3d();
        var endPoint = circularArc2d.EndPoint.ToRhinoPoint3d();

        // Vector has to be negated as the Rhino arc is drawn in the opposite direction.
        var tangentVector = circularArc2d.GetTangent(circularArc2d.StartPoint).Direction.Negate();
        var rhinoVector = tangentVector.ToRhinoVector3d();

        return new RhinoArc(startPoint, rhinoVector, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD EllipticalArc2d to a Rhino Ellipse with interval, applying unit conversion.
    /// </summary>
    /// <param name="ellipticalArc2d">The AutoCAD 2D elliptical arc to convert.</param>
    /// <param name="interval">The interval representing the arc portion of the ellipse.</param>
    /// <returns>A Rhino Ellipse with radii scaled to Rhino units.</returns>
    public static RhinoEllipse ToRhinoEllipse(this EllipticalArc2d ellipticalArc2d, out RhinoInterval interval)
    {
        var centrePoint = ellipticalArc2d.Center.ToRhinoPoint3d();
        var majorAxis = ellipticalArc2d.MajorAxis.ToRhinoVector3d();
        var minorAxis = ellipticalArc2d.MinorAxis.ToRhinoVector3d();

        var majorRadius = UnitConverter.ToRhinoLength(ellipticalArc2d.MajorRadius);
        var minorRadius = UnitConverter.ToRhinoLength(ellipticalArc2d.MinorRadius);

        var plane = new RhinoPlane(centrePoint, majorAxis, minorAxis);
        var rhinoEllipse = new RhinoEllipse(plane, majorRadius, minorRadius);

        var ellipticalArc2dInterval = ellipticalArc2d.GetInterval();
        interval = new RhinoInterval(ellipticalArc2dInterval.LowerBound, ellipticalArc2dInterval.UpperBound);

        return rhinoEllipse;
    }

    /// <summary>
    /// Converts an AutoCAD SplineEntity2d to a Rhino NurbsCurve, applying unit conversion.
    /// </summary>
    /// <param name="spline2d">The AutoCAD 2D spline to convert.</param>
    /// <returns>A Rhino NurbsCurve with control points scaled to Rhino units.</returns>
    public static RhinoNurbsCurve ToRhinoNurbsCurve(this SplineEntity2d spline2d)
    {
        var pointCount = spline2d.NumControlPoints;
        var rhinoPoints = new List<RhinoPoint3d>();

        for (var i = 0; i < pointCount; i++)
        {
            var controlPoint = spline2d.GetControlPointAt(i);
            var point3d = controlPoint.ToRhinoPoint3d();
            rhinoPoints.Add(point3d);
        }

        var isPeriodic = spline2d.IsPeriodic(out _);
        return RhinoNurbsCurve.CreateSubDFriendly(rhinoPoints, false, isPeriodic);
    }

    /// <summary>
    /// Converts an AutoCAD NurbCurve2d to a Rhino NurbsCurve, applying unit conversion.
    /// </summary>
    /// <param name="nurbCurve2d">The AutoCAD 2D NURBS curve to convert.</param>
    /// <returns>A Rhino NurbsCurve with control points scaled to Rhino units.</returns>
    public static RhinoNurbsCurve ToRhinoNurbsCurve(this NurbCurve2d nurbCurve2d)
    {
        return ((SplineEntity2d)nurbCurve2d).ToRhinoNurbsCurve();
    }

    /// <summary>
    /// Converts an AutoCAD CompositeCurve2d to a Rhino PolyCurve, applying unit conversion.
    /// </summary>
    /// <param name="compositeCurve2d">The AutoCAD 2D composite curve to convert.</param>
    /// <returns>A Rhino PolyCurve containing all converted segments.</returns>
    public static RhinoPolyCurve ToRhinoPolyCurve(this CompositeCurve2d compositeCurve2d)
    {
        var curves = compositeCurve2d.GetCurves();
        var rhinoPolyCurve = new RhinoPolyCurve();

        foreach (var curve2d in curves.OfType<Curve2d>())
        {
            var rhinoCurve = curve2d.ToRhinoCurve();
            if (rhinoCurve != null)
                rhinoPolyCurve.Append(rhinoCurve);
        }

        return rhinoPolyCurve;
    }

    /// <summary>
    /// Converts an AutoCAD Curve2d to the appropriate Rhino curve type.
    /// </summary>
    /// <param name="curve">The AutoCAD 2D curve to convert.</param>
    /// <returns>A Rhino Curve, or null if the curve type is not supported.</returns>
    public static Rhino.Geometry.Curve? ToRhinoCurve(this Curve2d curve)
    {
        switch (curve)
        {
            case LineSegment2d line:
                return line.ToRhinoLineCurve();

            case Line2d line2d:
                return line2d.ToRhinoLineCurve();

            case CircularArc2d circularArc2d:
                var arc = circularArc2d.ToRhinoArc();
                return new RhinoArcCurve(arc);

            case SplineEntity2d splineEntity2d:
                return splineEntity2d.ToRhinoNurbsCurve();

            case EllipticalArc2d ellipticalArc2d:
                var ellipse = ellipticalArc2d.ToRhinoEllipse(out var interval);
                var nurbs = ellipse.ToNurbsCurve();
                var nurbsTrimmed = nurbs.Trim(interval);
                var ellipseNurbs = nurbsTrimmed == null ? nurbs : (RhinoNurbsCurve)nurbsTrimmed;

                var endPoint = ellipticalArc2d.EndPoint.ToRhinoPoint3d();
                var startParameter = interval.T0;
                var endParameter = interval.T1;

                var samplePoints = ellipticalArc2d.GetSamplePoints(startParameter, endParameter, interval.Length).ToList();
                var hasSamplePoints = samplePoints.Count > 0;

                var endTangent = hasSamplePoints
                    ? samplePoints.Last().GetDerivative(1, endParameter).ToRhinoVector3d()
                    : new RhinoVector3d(0, 0, 0);

                ellipseNurbs.SetEndCondition(true, RhinoNurbsCurve.NurbsCurveEndConditionType.Position, endPoint, endTangent);
                return ellipseNurbs;

            default:
                return null;
        }
    }

    /// <summary>
    /// Adjusts knot vector to match Rhino's requirements.
    /// The number of knots must be D + N - 1, where D = degree and N = number of control points.
    /// AutoCAD may use D + N + 1 knots, so this method adjusts multiplicity as needed.
    /// </summary>
    private static List<double> GetKnots(List<double> inputKnots, int degree, int numberOfControlPoints)
    {
        var targetNumberOfKnots = degree + numberOfControlPoints - 1;

        if (inputKnots.Count == targetNumberOfKnots)
            return inputKnots;

        var outputKnots = new List<double>(targetNumberOfKnots);
        var currentKnotValue = inputKnots[0];
        var currentValueCount = 1;
        outputKnots.Add(currentKnotValue);

        // Skip first as already added
        for (var index = 1; index < inputKnots.Count; index++)
        {
            var inputKnot = inputKnots[index];
            if (Math.Abs(currentKnotValue - inputKnot) > GeometryConstants.ZeroTolerance)
            {
                currentKnotValue = inputKnot;
                currentValueCount = 1;
                outputKnots.Add(inputKnot);
                continue;
            }

            currentValueCount++;

            if (currentValueCount > degree)
                continue;

            outputKnots.Add(inputKnot);
        }

        return outputKnots;
    }

    /// <summary>
    /// Converts an AutoCAD database Curve to the appropriate Rhino curve type.
    /// </summary>
    /// <param name="curve">The AutoCAD curve to convert.</param>
    /// <returns>A Rhino Curve, or null if the curve type is not supported.</returns>
    public static RhinoCurve? ToRhinoCurve(this CadCurve curve)
    {
        switch (curve)
        {
            case CadLine line:
                return line.ToRhinoLineCurve();

            case Spline spline:
                return spline.ToRhinoNurbsCurve();

            case CadEllipse ellipse:
                var rhinoEllipse = ellipse.ToRhinoEllipse();
                return rhinoEllipse.ToNurbsCurve();

            case CadArc arc:
                var rhinoArc = arc.ToRhinoArc();
                return rhinoArc.ToNurbsCurve();

            case CadCircle circle:
                var rhinoCircle = circle.ToRhinoCircle();
                return rhinoCircle.ToNurbsCurve();

            case CadPolyline polyline:
                return polyline.ToRhinoPolyCurve();

            default:
                return null;
        }
    }

    /// <summary>
    /// Converts an AutoCAD Polyline to a Rhino PolyCurve.
    /// </summary>
    /// <param name="polyline">The AutoCAD polyline to convert.</param>
    /// <returns>A Rhino PolyCurve containing line and arc segments.</returns>
    public static RhinoPolyCurve ToRhinoPolyCurve(this CadPolyline polyline)
    {
        var vertexCount = polyline.NumberOfVertices;
        var polyCurve = new RhinoPolyCurve();

        for (var index = 0; index < vertexCount; index++)
        {
            var segmentType = polyline.GetSegmentType(index);

            switch (segmentType)
            {
                case SegmentType.Line:
                    {
                        var lineSegment2d = polyline.GetLineSegment2dAt(index);
                        var lineCurve = lineSegment2d.ToRhinoLineCurve();
                        polyCurve.Append(lineCurve);
                        break;
                    }
                case SegmentType.Arc:
                    {
                        var arcSegment2d = polyline.GetArcSegment2dAt(index);
                        var arc = arcSegment2d.ToRhinoArc();
                        var arcCurve = new RhinoArcCurve(arc);
                        polyCurve.Append(arcCurve);
                        break;
                    }
                default:
                    continue;
            }
        }

        return polyCurve;
    }

    /// <summary>
    /// Converts an AutoCAD DBPoint to a Rhino Point.
    /// </summary>
    /// <param name="point">The AutoCAD point to convert.</param>
    /// <returns>A Rhino Point with coordinates scaled to Rhino units.</returns>
    public static RhinoPoint ToRhinoPoint(this CadDBPoint point)
    {
        var point3d = point.Position.ToRhinoPoint3d();
        return new RhinoPoint(point3d);
    }

    /// <summary>
    /// Converts an AutoCAD Curve2dCollection to a Rhino PolyCurve.
    /// </summary>
    /// <param name="cadCurveCollection">The AutoCAD 2D curve collection to convert.</param>
    /// <returns>A Rhino PolyCurve containing all converted curves.</returns>
    public static RhinoPolyCurve ToRhinoPolyCurve(this Curve2dCollection cadCurveCollection)
    {
        var rhinoPolyCurve = new RhinoPolyCurve();

        foreach (var curve2d in cadCurveCollection.OfType<Curve2d>())
        {
            var internalCurve = curve2d.ToRhinoCurve();
            if (internalCurve != null)
                rhinoPolyCurve.Append(internalCurve);
        }

        return rhinoPolyCurve;
    }

    /// <summary>
    /// Converts an AutoCAD BulgeVertexCollection to a Rhino PolyCurve.
    /// </summary>
    /// <param name="bulgeVertexCollection">The AutoCAD bulge vertex collection to convert.</param>
    /// <returns>A Rhino PolyCurve containing line and arc segments.</returns>
    public static RhinoPolyCurve ToRhinoPolyCurve(this BulgeVertexCollection bulgeVertexCollection)
    {
        var polyline = new CadPolyline();

        for (var index = 0; index < bulgeVertexCollection.Count; index++)
        {
            var vertex = bulgeVertexCollection[index];
            polyline.AddVertexAt(index, vertex.Vertex, vertex.Bulge, 0.0, 0.0);
        }

        return polyline.ToRhinoPolyCurve();
    }
}
