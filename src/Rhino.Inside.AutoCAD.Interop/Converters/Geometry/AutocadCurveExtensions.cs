using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Diagnostics;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using CadPolyline2d = Autodesk.AutoCAD.DatabaseServices.Polyline2d;
using CadPolyline3d = Autodesk.AutoCAD.DatabaseServices.Polyline3d;
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
using RhinoPolylineCurve = Rhino.Geometry.PolylineCurve;
using RhinoTransform = Rhino.Geometry.Transform;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Extension methods for converting AutoCAD curve types to their Rhino equivalents.
/// </summary>
/// <remarks>
/// All conversion methods apply unit scaling via <see cref="UnitConverter"/> to ensure
/// geometric data is correctly transformed between AutoCAD and Rhino coordinate systems.
/// </remarks>
/// <seealso cref="RhinoCurveExtensions"/>
/// <seealso cref="AutocadGeometryExtensions"/>
public static class AutocadCurveExtensions
{
    /// <summary>
    /// Converts an AutoCAD <see cref="CadLine"/> to a Rhino <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <param name="line">
    /// The AutoCAD line to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoLineCurve"/> with endpoints scaled to Rhino units.
    /// </returns>
    /// <seealso cref="ToRhinoLineCurve(LineSegment2d)"/>
    /// <seealso cref="ToRhinoLineCurve(LineSegment3d)"/>
    public static RhinoLineCurve ToRhinoLineCurve(this CadLine line)
    {
        var startPoint = line.StartPoint.ToRhinoPoint3d();
        var endPoint = line.EndPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadArc"/> to a Rhino <see cref="RhinoArc"/>.
    /// </summary>
    /// <param name="arc">
    /// The AutoCAD arc to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoArc"/> with radius and plane scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// The arc's plane is derived from <see cref="CadArc.GetPlane()"/>, preserving the
    /// original orientation. Start angle and sweep angle are transferred directly.
    /// </remarks>
    /// <seealso cref="ToRhinoArc(CircularArc2d)"/>
    /// <seealso cref="ToRhinoArc(CircularArc3d)"/>
    public static RhinoArc ToRhinoArc(this CadArc arc)
    {
        var plane = arc.GetPlane().ToRhinoPlane();
        var radius = UnitConverter.ToRhinoLength(arc.Radius);
        var sweepAngle = arc.TotalAngle;

        var rhinoArc = new RhinoArc(plane, radius, sweepAngle)
        {
            StartAngle = arc.StartAngle,
            Angle = sweepAngle
        };

        return rhinoArc;
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadCircle"/> to a Rhino <see cref="RhinoCircle"/>.
    /// </summary>
    /// <param name="circle">
    /// The AutoCAD circle to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoCircle"/> centered at the converted origin with radius scaled to Rhino units.
    /// </returns>
    public static RhinoCircle ToRhinoCircle(this CadCircle circle)
    {
        var origin = circle.Center.ToRhinoPoint3d();
        var radius = UnitConverter.ToRhinoLength(circle.Radius);
        return new RhinoCircle(origin, radius);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadEllipse"/> to a Rhino <see cref="RhinoEllipse"/>.
    /// </summary>
    /// <param name="ellipse">
    /// The AutoCAD ellipse to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoEllipse"/> with both radii scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// The ellipse plane is constructed from the center point and major/minor axis vectors,
    /// preserving the original 3D orientation of the ellipse.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(CadEllipse)"/>
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
    /// Converts an AutoCAD <see cref="Spline"/> to a Rhino <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="spline">
    /// The AutoCAD spline to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> with control points scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// Control points and weights are transferred from the spline's NURBS data.
    /// The knot vector is normalized via <see cref="GetValidKnots"/> to match Rhino's
    /// expected format. The curve is trimmed to the original parameter domain.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(SplineEntity2d)"/>
    /// <seealso cref="ToRhinoNurbsCurve(SplineEntity3d)"/>
    public static RhinoNurbsCurve ToRhinoNurbsCurve(this Spline spline)
    {
        var nurbsData = spline.NurbsData;
        var point3dCollection = nurbsData.GetControlPoints();
        var weights = nurbsData.GetWeights();
        var weightsCount = weights.Count;

        var nurbsDataKnots = nurbsData.GetKnots().ToArray().ToList();
        var knots = GetValidKnots(nurbsDataKnots, nurbsData.Degree, point3dCollection.Count);

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
    /// Converts an AutoCAD <see cref="LineSegment2d"/> to a Rhino <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <param name="lineSegment2d">
    /// The AutoCAD 2D line segment to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoLineCurve"/> with endpoints scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// The 2D endpoints are converted using <see cref="AutocadGeometryExtensions.ToRhinoPoint2d"/>,
    /// resulting in a line curve on the XY plane.
    /// </remarks>
    /// <seealso cref="ToRhinoLineCurve(CadLine)"/>
    /// <seealso cref="ToRhinoLineCurve(LineSegment3d)"/>
    public static RhinoLineCurve ToRhinoLineCurve(this LineSegment2d lineSegment2d)
    {
        var startPoint = lineSegment2d.StartPoint.ToRhinoPoint2d();
        var endPoint = lineSegment2d.EndPoint.ToRhinoPoint2d();
        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="LineSegment3d"/> to a Rhino <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <param name="lineSegment3d">
    /// The AutoCAD 3D line segment to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoLineCurve"/> with endpoints scaled to Rhino units.
    /// </returns>
    /// <seealso cref="ToRhinoLineCurve(CadLine)"/>
    /// <seealso cref="ToRhinoLineCurve(LineSegment2d)"/>
    public static RhinoLineCurve ToRhinoLineCurve(this LineSegment3d lineSegment3d)
    {
        var startPoint = lineSegment3d.StartPoint.ToRhinoPoint3d();
        var endPoint = lineSegment3d.EndPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="Line2d"/> to a Rhino <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <param name="line2d">
    /// The AutoCAD 2D line to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoLineCurve"/> representing the line segment.
    /// </returns>
    /// <remarks>
    /// AutoCAD <see cref="Line2d"/> objects can be infinite, which Rhino does not support.
    /// When endpoints are not defined:
    /// <list type="bullet">
    ///   <item>
    ///     <see cref="Line2d.PointOnLine"/> is used as the start point
    ///   </item>
    ///   <item>
    ///     The endpoint is computed by translating the start point along the direction vector
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ToRhinoLineCurve(Line3d)"/>
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
    /// Converts an AutoCAD <see cref="Line3d"/> to a Rhino <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <param name="line3d">
    /// The AutoCAD 3D line to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoLineCurve"/> representing the line segment.
    /// </returns>
    /// <remarks>
    /// AutoCAD <see cref="Line3d"/> objects can be infinite, which Rhino does not support.
    /// When endpoints are not defined:
    /// <list type="bullet">
    ///   <item>
    ///     <see cref="Line3d.PointOnLine"/> is used as the start point
    ///   </item>
    ///   <item>
    ///     The endpoint is computed by translating the start point along the direction vector
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ToRhinoLineCurve(Line2d)"/>
    public static RhinoLineCurve ToRhinoLineCurve(this Line3d line3d)
    {
        var hasStartPoint = line3d.HasStartPoint;
        var startPoint = hasStartPoint ? line3d.StartPoint.ToRhinoPoint3d() : line3d.PointOnLine.ToRhinoPoint3d();

        var hasEndPoint = line3d.HasEndPoint;
        RhinoPoint3d endPoint;

        if (hasEndPoint)
        {
            endPoint = line3d.EndPoint.ToRhinoPoint3d();
        }
        else
        {
            var vector3d = line3d.Direction.ToRhinoVector3d();
            var translation = Rhino.Geometry.Transform.Translation(vector3d);
            endPoint = new RhinoPoint3d(startPoint);
            endPoint.Transform(translation);
        }

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CircularArc2d"/> to a Rhino <see cref="RhinoArc"/>.
    /// </summary>
    /// <param name="circularArc2d">
    /// The AutoCAD 2D circular arc to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoArc"/> constructed from the start point, tangent direction, and end point.
    /// </returns>
    /// <remarks>
    /// The tangent vector at the start point is negated because Rhino arcs
    /// are drawn in the opposite direction compared to AutoCAD arcs.
    /// </remarks>
    /// <seealso cref="ToRhinoArc(CadArc)"/>
    /// <seealso cref="ToRhinoArc(CircularArc3d)"/>
    public static RhinoArc ToRhinoArc(this CircularArc2d circularArc2d)
    {
        var startPoint = circularArc2d.StartPoint.ToRhinoPoint3d();
        var endPoint = circularArc2d.EndPoint.ToRhinoPoint3d();

        var tangentVector = circularArc2d.GetTangent(circularArc2d.StartPoint).Direction.Negate();
        var rhinoVector = tangentVector.ToRhinoVector3d();

        return new RhinoArc(startPoint, rhinoVector, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CircularArc3d"/> to a Rhino <see cref="RhinoArc"/>.
    /// </summary>
    /// <param name="circularArc3d">
    /// The AutoCAD 3D circular arc to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoArc"/> constructed from the start point, tangent direction, and end point.
    /// </returns>
    /// <remarks>
    /// The tangent vector at the start point is negated because Rhino arcs
    /// are drawn in the opposite direction compared to AutoCAD arcs.
    /// </remarks>
    /// <seealso cref="ToRhinoArc(CadArc)"/>
    /// <seealso cref="ToRhinoArc(CircularArc2d)"/>
    public static RhinoArc ToRhinoArc(this CircularArc3d circularArc3d)
    {
        var startPoint = circularArc3d.StartPoint.ToRhinoPoint3d();
        var endPoint = circularArc3d.EndPoint.ToRhinoPoint3d();

        var tangentVector = circularArc3d.GetTangent(circularArc3d.StartPoint).Direction.Negate();
        var rhinoVector = tangentVector.ToRhinoVector3d();

        return new RhinoArc(startPoint, rhinoVector, endPoint);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="SplineEntity2d"/> to a Rhino <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="spline2d">
    /// The AutoCAD 2D spline to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> with control points scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// Creates a SubD-friendly NURBS curve from the control points.
    /// The periodicity of the source spline is preserved.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(Spline)"/>
    /// <seealso cref="ToRhinoNurbsCurve(SplineEntity3d)"/>
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
    /// Converts an AutoCAD <see cref="SplineEntity3d"/> to a Rhino <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="spline3d">
    /// The AutoCAD 3D spline to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> with control points scaled to Rhino units.
    /// </returns>
    /// <remarks>
    /// Creates a SubD-friendly NURBS curve from the control points.
    /// The periodicity of the source spline is preserved.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(Spline)"/>
    /// <seealso cref="ToRhinoNurbsCurve(SplineEntity2d)"/>
    public static RhinoNurbsCurve ToRhinoNurbsCurve(this SplineEntity3d spline3d)
    {
        var pointCount = spline3d.NumberOfControlPoints;
        var rhinoPoints = new List<RhinoPoint3d>();

        for (var i = 0; i < pointCount; i++)
        {
            var controlPoint = spline3d.ControlPointAt(i);
            var point3d = controlPoint.ToRhinoPoint3d();
            rhinoPoints.Add(point3d);
        }

        var isPeriodic = spline3d.IsPeriodic(out _);
        return RhinoNurbsCurve.CreateSubDFriendly(rhinoPoints, false, isPeriodic);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CompositeCurve2d"/> to a Rhino <see cref="RhinoPolyCurve"/>.
    /// </summary>
    /// <param name="compositeCurve2d">
    /// The AutoCAD 2D composite curve to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoPolyCurve"/> containing all converted curve segments.
    /// </returns>
    /// <remarks>
    /// Each segment in the composite curve is individually converted using <see cref="ToRhinoCurve(Curve2d)"/>
    /// and appended to the resulting polycurve. Unsupported segment types are skipped.
    /// </remarks>
    /// <seealso cref="ToRhinoPolyCurve(CadPolyline)"/>
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
    /// Converts an AutoCAD <see cref="Curve2d"/> to the appropriate Rhino curve type.
    /// </summary>
    /// <param name="curve">
    /// The AutoCAD 2D curve to convert.
    /// </param>
    /// <returns>
    /// A Rhino <see cref="Rhino.Geometry.Curve"/>, or <see langword="null"/> if the curve type is not supported.
    /// </returns>
    /// <remarks>
    /// Supported types include:
    /// <list type="bullet">
    ///   <item><see cref="LineSegment2d"/> and <see cref="Line2d"/></item>
    ///   <item><see cref="CircularArc2d"/></item>
    ///   <item><see cref="SplineEntity2d"/></item>
    ///   <item><see cref="EllipticalArc2d"/></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ToRhinoCurve(Curve3d)"/>
    /// <seealso cref="ToRhinoCurve(CadCurve)"/>
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
                return ellipticalArc2d.ToRhinoNurbsCurve();

            default:
                return null;
        }
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="Curve3d"/> to the appropriate Rhino curve type.
    /// </summary>
    /// <param name="curve">
    /// The AutoCAD 3D curve to convert.
    /// </param>
    /// <returns>
    /// A Rhino <see cref="Rhino.Geometry.Curve"/>, or <see langword="null"/> if the curve type is not supported.
    /// </returns>
    /// <remarks>
    /// Supported types include:
    /// <list type="bullet">
    ///   <item><see cref="LineSegment3d"/> and <see cref="Line3d"/></item>
    ///   <item><see cref="CircularArc3d"/></item>
    ///   <item><see cref="SplineEntity3d"/></item>
    ///   <item><see cref="EllipticalArc3d"/></item>
    ///   <item><see cref="ExternalCurve3d"/> (recursively converts native curve)</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ToRhinoCurve(Curve2d)"/>
    /// <seealso cref="ToRhinoCurve(CadCurve)"/>
    public static Rhino.Geometry.Curve? ToRhinoCurve(this Curve3d curve)
    {
        switch (curve)
        {
            case LineSegment3d line:
                return line.ToRhinoLineCurve();

            case Line3d line2d:
                return line2d.ToRhinoLineCurve();

            case CircularArc3d circularArc2d:
                var arc = circularArc2d.ToRhinoArc();
                return new RhinoArcCurve(arc);

            case SplineEntity3d splineEntity2d:
                return splineEntity2d.ToRhinoNurbsCurve();

            case EllipticalArc3d ellipticalArc2d:
                return ellipticalArc2d.ToRhinoNurbsCurve();

            case ExternalCurve3d externalCurve3d:
                var nurbsCurve = externalCurve3d.NativeCurve;
                return nurbsCurve?.ToRhinoCurve();
            default:
                return null;
        }
    }

    /// <summary>
    /// Converts an <see cref="EllipticalArc2d"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="ellipticalArc2d">
    /// The source elliptical arc to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> representing the elliptical arc,
    /// or <see langword="null"/> if the NURBS curve could not be created.
    /// </returns>
    /// <remarks>
    /// The conversion uses a two-step approach:
    /// <list type="number">
    ///   <item>
    ///     A circular arc is constructed using the major radius, oriented on a plane
    ///     derived from the ellipse's major and minor axes
    ///   </item>
    ///   <item>
    ///     A non-uniform scale transform squashes the circle into the correct elliptical
    ///     shape using the minor-to-major radius ratio
    ///   </item>
    /// </list>
    /// Unit conversion is applied via <see cref="UnitConverter.ToRhinoLength"/>.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(EllipticalArc3d)"/>
    /// <seealso cref="ToRhinoNurbsCurve(CadEllipse)"/>
    public static RhinoNurbsCurve? ToRhinoNurbsCurve(this EllipticalArc2d ellipticalArc2d)
    {
        var center = ellipticalArc2d.Center.ToRhinoPoint3d();

        var majorRadius = UnitConverter.ToRhinoLength(ellipticalArc2d.MajorRadius);

        var manorRadius = UnitConverter.ToRhinoLength(ellipticalArc2d.MinorRadius);

        var startAngle = ellipticalArc2d.StartAngle;

        var endAngle = ellipticalArc2d.EndAngle;

        var majorDir = ellipticalArc2d.MajorAxis.ToRhinoVector3d();
        majorDir.Unitize();

        var minorDirection = ellipticalArc2d.MinorAxis.ToRhinoVector3d();
        minorDirection.Unitize();

        var plane = new RhinoPlane(center, majorDir, minorDirection);

        var circle = new RhinoCircle(plane, majorRadius);

        var arc = new RhinoArc(circle, new RhinoInterval(startAngle, endAngle));

        var nurbsCurve = RhinoNurbsCurve.CreateFromArc(arc);

        if (nurbsCurve == null)
            return null;

        var radiusRatio = manorRadius / majorRadius;
        var transform = RhinoTransform.Scale(
            plane,
            1.0,
            radiusRatio,
            1.0
        );

        nurbsCurve.Transform(transform);

        return nurbsCurve;
    }

    /// <summary>
    /// Converts an <see cref="EllipticalArc3d"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="ellipticalArc3d">
    /// The source elliptical arc to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> representing the elliptical arc,
    /// or <see langword="null"/> if the NURBS curve could not be created.
    /// </returns>
    /// <remarks>
    /// The conversion uses a two-step approach:
    /// <list type="number">
    ///   <item>
    ///     A circular arc is constructed using the major radius, oriented on a plane
    ///     derived from the ellipse's major and minor axes
    ///   </item>
    ///   <item>
    ///     A non-uniform scale transform squashes the circle into the correct elliptical
    ///     shape using the minor-to-major radius ratio
    ///   </item>
    /// </list>
    /// Unit conversion is applied via <see cref="UnitConverter.ToRhinoLength"/>.
    /// </remarks>
    /// <seealso cref="ToRhinoNurbsCurve(EllipticalArc2d)"/>
    /// <seealso cref="ToRhinoNurbsCurve(CadEllipse)"/>
    public static RhinoNurbsCurve? ToRhinoNurbsCurve(this EllipticalArc3d ellipticalArc3d)
    {
        var center = ellipticalArc3d.Center.ToRhinoPoint3d();

        var majorRadius = UnitConverter.ToRhinoLength(ellipticalArc3d.MajorRadius);

        var manorRadius = UnitConverter.ToRhinoLength(ellipticalArc3d.MinorRadius);

        var startAngle = ellipticalArc3d.StartAngle;

        var endAngle = ellipticalArc3d.EndAngle;

        var majorDir = ellipticalArc3d.MajorAxis.ToRhinoVector3d();
        majorDir.Unitize();

        var minorDirection = ellipticalArc3d.MinorAxis.ToRhinoVector3d();
        minorDirection.Unitize();

        var plane = new RhinoPlane(center, majorDir, minorDirection);

        var circle = new RhinoCircle(plane, majorRadius);

        var arc = new RhinoArc(circle, new RhinoInterval(startAngle, endAngle));

        var nurbsCurve = RhinoNurbsCurve.CreateFromArc(arc);

        if (nurbsCurve == null)
            return null;

        var radiusRatio = manorRadius / majorRadius;
        var transform = RhinoTransform.Scale(
            plane,
            1.0,
            radiusRatio,
            1.0
        );

        nurbsCurve.Transform(transform);

        return nurbsCurve;
    }

    /// <summary>
    /// Converts an AutoCAD database <see cref="CadEllipse"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    /// <param name="ellipse">
    /// The AutoCAD ellipse entity to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoNurbsCurve"/> representing the ellipse,
    /// or <see langword="null"/> if the NURBS curve could not be created.
    /// </returns>
    /// <remarks>
    /// The conversion uses a two-step approach:
    /// <list type="number">
    ///   <item>
    ///     A circular arc is constructed using the major radius, oriented on a plane
    ///     derived from the ellipse's major and minor axes
    ///   </item>
    ///   <item>
    ///     A non-uniform scale transform squashes the circle into the correct elliptical
    ///     shape using the minor-to-major radius ratio
    ///   </item>
    /// </list>
    /// Unit conversion is applied via <see cref="UnitConverter.ToRhinoLength"/>.
    /// </remarks>
    /// <seealso cref="ToRhinoEllipse"/>
    /// <seealso cref="ToRhinoNurbsCurve(EllipticalArc2d)"/>
    /// <seealso cref="ToRhinoNurbsCurve(EllipticalArc3d)"/>
    public static RhinoNurbsCurve? ToRhinoNurbsCurve(this CadEllipse ellipse)
    {
        var center = ellipse.Center.ToRhinoPoint3d();

        var majorRadius = UnitConverter.ToRhinoLength(ellipse.MajorRadius);

        var manorRadius = UnitConverter.ToRhinoLength(ellipse.MinorRadius);

        var startAngle = ellipse.StartAngle;

        var endAngle = ellipse.EndAngle;

        var majorDir = ellipse.MajorAxis.ToRhinoVector3d();
        majorDir.Unitize();

        var minorDirection = ellipse.MinorAxis.ToRhinoVector3d();
        minorDirection.Unitize();

        var plane = new RhinoPlane(center, majorDir, minorDirection);

        var circle = new RhinoCircle(plane, majorRadius);

        var arc = new RhinoArc(circle, new RhinoInterval(startAngle, endAngle));

        var nurbsCurve = RhinoNurbsCurve.CreateFromArc(arc);

        if (nurbsCurve == null)
            return null;

        var radiusRatio = manorRadius / majorRadius;
        var transform = RhinoTransform.Scale(
            plane,
            1.0,
            radiusRatio,
            1.0
        );

        nurbsCurve.Transform(transform);

        return nurbsCurve;
    }

    /// <summary>
    /// Produces a valid knot vector for Rhino's NURBS representation from a source knot vector.
    /// </summary>
    /// <param name="inputKnots">
    /// A non-empty, non-decreasing sequence of knot values from the source system.
    /// </param>
    /// <param name="degree">
    /// The polynomial degree of the NURBS curve.
    /// </param>
    /// <param name="numberOfControlPoints">
    /// The number of control points on the curve.
    /// </param>
    /// <returns>
    /// A knot vector containing exactly <c>degree + numberOfControlPoints - 1</c> values.
    /// </returns>
    /// <remarks>
    /// AutoCAD uses a D + N + 1 knot convention while Rhino uses D + N - 1.
    /// This method normalizes the knot vector by:
    /// <list type="bullet">
    ///   <item>Capping per-knot multiplicity at the degree value</item>
    ///   <item>Preserving the relative order of knots</item>
    ///   <item>Using <see cref="GeometryConstants.ZeroTolerance"/> for duplicate detection</item>
    /// </list>
    /// </remarks>
    public static List<double> GetValidKnots(
        List<double> inputKnots,
        int degree,
        int numberOfControlPoints)
    {
        var targetCount = degree + numberOfControlPoints - 1;

        if (inputKnots.Count == targetCount)
            return inputKnots;

        var output = new List<double>(targetCount);

        var currentMultiplicity = 0;

        double? previousKnot = null;

        foreach (var knot in inputKnots)
        {
            if (output.Count == targetCount)
                break;

            if (previousKnot is null ||
                Math.Abs(knot - previousKnot.Value) > GeometryConstants.ZeroTolerance)
            {
                currentMultiplicity = 1;

                previousKnot = knot;

                output.Add(knot);

                continue;
            }

            if (currentMultiplicity >= degree) continue;

            currentMultiplicity++;

            output.Add(knot);

        }

        return output;
    }

    /// <summary>
    /// Converts an AutoCAD database <see cref="CadCurve"/> to the appropriate Rhino curve type.
    /// </summary>
    /// <param name="curve">
    /// The AutoCAD database curve entity to convert.
    /// </param>
    /// <returns>
    /// A Rhino <see cref="RhinoCurve"/>, or <see langword="null"/> if the curve type is not supported.
    /// </returns>
    /// <remarks>
    /// This is the primary entry point for converting AutoCAD database curve entities.
    /// Supported types include:
    /// <list type="bullet">
    ///   <item><see cref="CadLine"/></item>
    ///   <item><see cref="Spline"/></item>
    ///   <item><see cref="CadEllipse"/></item>
    ///   <item><see cref="CadArc"/></item>
    ///   <item><see cref="CadCircle"/></item>
    ///   <item><see cref="CadPolyline"/></item>
    ///   <item><see cref="CadPolyline2d"/></item>
    ///   <item><see cref="CadPolyline3d"/></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ToRhinoCurve(Curve2d)"/>
    /// <seealso cref="ToRhinoCurve(Curve3d)"/>
    public static RhinoCurve? ToRhinoCurve(this CadCurve curve)
    {
        switch (curve)
        {
            case CadLine line:
                return line.ToRhinoLineCurve();

            case Spline spline:
                return spline.ToRhinoNurbsCurve();

            case CadEllipse ellipse:
                return ellipse.ToRhinoNurbsCurve();

            case CadArc arc:
                var rhinoArc = arc.ToRhinoArc();
                return rhinoArc.ToNurbsCurve();

            case CadCircle circle:
                var rhinoCircle = circle.ToRhinoCircle();
                return rhinoCircle.ToNurbsCurve();

            case CadPolyline polyline:
                return polyline.ToRhinoPolyCurve();
            case CadPolyline2d polyline:
                return polyline.ToRhinoCurve();
            case CadPolyline3d polyline:
                return polyline.ToRhinoCurve();

            default:
                Debug.WriteLine(curve.GetType().Name);
                return null;
        }
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadPolyline"/> to a Rhino <see cref="RhinoPolyCurve"/>.
    /// </summary>
    /// <param name="polyline">
    /// The AutoCAD polyline to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoPolyCurve"/> containing line and arc segments.
    /// </returns>
    /// <remarks>
    /// Iterates through each vertex and extracts the appropriate segment type
    /// (<see cref="SegmentType.Line"/> or <see cref="SegmentType.Arc"/>).
    /// Other segment types are skipped.
    /// </remarks>
    /// <seealso cref="ToRhinoPolyCurve(CompositeCurve2d)"/>
    /// <seealso cref="ToRhinoPolyCurve(Curve2dCollection)"/>
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
    /// Converts an AutoCAD polyline2d to a Rhino curve.
    /// </summary>
    public static RhinoCurve? ToRhinoCurve(this Polyline2d polyline2d)
    {
        // A non-database-resident polyline (e.g. a clone or a newly created entity)
        // enumerates its vertices directly instead of yielding ObjectIds, and has no
        // document to lock.
        if (polyline2d.Database == null)
        {
            var points = new List<RhinoPoint3d>();

            foreach (var item in polyline2d)
            {
                if (item is Vertex2d vertex)
                {
                    var rhinoPoint = vertex.Position.ToRhinoPoint3d();
                    points.Add(rhinoPoint);
                }
            }

            return points.ToRhinoPolylineCurve();
        }

        var activeDocument = Application.DocumentManager.GetDocument(polyline2d.Database);

        using var documentLock = activeDocument.LockDocument();

        var transactionManagerWrapper = new AutocadTransactionManagerWrapper(activeDocument);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = polyline2d.ToRhinoCurve(transactionManagerWrapper);

        transaction.Commit();
        return result;
    }

    /// <summary>
    /// Converts an AutoCAD polyline2d to a Rhino curve.
    /// </summary>
    public static RhinoCurve? ToRhinoCurve(this Polyline2d polyline2d,
        IAutocadTransactionManager transactionManager)
    {
        var points = new List<RhinoPoint3d>();

        foreach (ObjectId vertexId in polyline2d)
        {
            if (transactionManager.Unwrap().GetObject(vertexId, OpenMode.ForRead) is Vertex2d vertex)
            {
                var rhinoPoint = vertex.Position.ToRhinoPoint3d();
                points.Add(rhinoPoint);
            }
        }

        return points.ToRhinoPolylineCurve();
    }

    /// <summary>
    /// Converts an AutoCAD Polyline3d to a Rhino curve.
    /// </summary>
    public static RhinoCurve? ToRhinoCurve(this Polyline3d polyline3d)
    {
        // A non-database-resident polyline (e.g. a clone or a newly created entity)
        // enumerates its vertices directly instead of yielding ObjectIds, and has no
        // document to lock.
        if (polyline3d.Database == null)
        {
            var points = new List<RhinoPoint3d>();

            foreach (var item in polyline3d)
            {
                if (item is PolylineVertex3d vertex)
                {
                    var rhinoPoint = vertex.Position.ToRhinoPoint3d();
                    points.Add(rhinoPoint);
                }
            }

            return points.ToRhinoPolylineCurve();
        }

        var activeDocument = Application.DocumentManager.GetDocument(polyline3d.Database);

        using var documentLock = activeDocument.LockDocument();

        var transactionManagerWrapper = new AutocadTransactionManagerWrapper(activeDocument);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = polyline3d.ToRhinoCurve(transactionManagerWrapper);

        transaction.Commit();
        return result;
    }

    /// <summary>
    /// Converts an AutoCAD Polyline3d to a Rhino curve.
    /// </summary>
    public static RhinoCurve? ToRhinoCurve(this Polyline3d polyline3d,
        IAutocadTransactionManager transactionManager)
    {
        var points = new List<RhinoPoint3d>();

        foreach (ObjectId vertexId in polyline3d)
        {
            if (transactionManager.Unwrap().GetObject(vertexId, OpenMode.ForRead) is PolylineVertex3d vertex)
            {
                var rhinoPoint = vertex.Position.ToRhinoPoint3d();
                points.Add(rhinoPoint);
            }
        }

        return points.ToRhinoPolylineCurve();
    }

    /// <summary>
    /// Creates a Rhino polyline curve from the given points, or null if there are
    /// fewer than two points.
    /// </summary>
    private static RhinoCurve? ToRhinoPolylineCurve(this List<RhinoPoint3d> points)
    {
        if (points.Count < 2)
            return null;

        return new RhinoPolylineCurve(points);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadDBPoint"/> to a Rhino <see cref="RhinoPoint"/>.
    /// </summary>
    /// <param name="point">
    /// The AutoCAD database point to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoPoint"/> with coordinates scaled to Rhino units.
    /// </returns>
    /// <seealso cref="AutocadGeometryExtensions.ToRhinoPoint3d"/>
    public static RhinoPoint ToRhinoPoint(this CadDBPoint point)
    {
        var point3d = point.Position.ToRhinoPoint3d();
        return new RhinoPoint(point3d);
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="Curve2dCollection"/> to a Rhino <see cref="RhinoPolyCurve"/>.
    /// </summary>
    /// <param name="cadCurveCollection">
    /// The AutoCAD 2D curve collection to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoPolyCurve"/> containing all successfully converted curves.
    /// </returns>
    /// <remarks>
    /// Each curve in the collection is converted using <see cref="ToRhinoCurve(Curve2d)"/>.
    /// Curves that fail conversion (return <see langword="null"/>) are skipped.
    /// </remarks>
    /// <seealso cref="ToRhinoPolyCurve(CadPolyline)"/>
    /// <seealso cref="ToRhinoPolyCurve(BulgeVertexCollection)"/>
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
    /// Converts an AutoCAD <see cref="BulgeVertexCollection"/> to a Rhino <see cref="RhinoPolyCurve"/>.
    /// </summary>
    /// <param name="bulgeVertexCollection">
    /// The AutoCAD bulge vertex collection to convert.
    /// </param>
    /// <returns>
    /// A <see cref="RhinoPolyCurve"/> containing line and arc segments derived from bulge values.
    /// </returns>
    /// <remarks>
    /// Bulge vertices are first assembled into a temporary <see cref="CadPolyline"/>,
    /// which is then converted using <see cref="ToRhinoPolyCurve(CadPolyline)"/>.
    /// The bulge value determines whether each segment is a line (bulge = 0) or arc (bulge != 0).
    /// </remarks>
    /// <seealso cref="ToRhinoPolyCurve(CadPolyline)"/>
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
