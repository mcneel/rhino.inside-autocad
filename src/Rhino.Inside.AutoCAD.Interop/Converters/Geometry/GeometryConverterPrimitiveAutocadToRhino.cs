using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Geometry;
using Arc = Autodesk.AutoCAD.DatabaseServices.Arc;
using Curve = Autodesk.AutoCAD.DatabaseServices.Curve;
using Ellipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using Line = Autodesk.AutoCAD.DatabaseServices.Line;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoVector2d = Rhino.Geometry.Vector2d;
using RhinoVector3d = Rhino.Geometry.Vector3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// This class handles conversion of primitive geometry types between AutoCAD and Rhino.
/// </summary>
public partial class GeometryConverter
{

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Geometry.Point2d"/> to a <see cref="RhinoPoint3d"/> with
    /// an optional input to provide the z coordinate.
    /// </summary>
    public RhinoPoint3d ConvertTo3d(Autodesk.AutoCAD.Geometry.Point2d point2d, double z = 0.0)
    {
        var rhinoPoint2d = this.ToRhinoType(point2d);

        return new RhinoPoint3d(rhinoPoint2d.X, rhinoPoint2d.Y, z);
    }

    /// <summary>
    /// Converts a <see cref="Vector2d"/> to a <see cref="RhinoVector3d"/> with
    /// an optional input to provide the z coordinate. The vector is normalized
    /// after creation.
    /// </summary>
    public RhinoVector3d ConvertTo3d(Autodesk.AutoCAD.Geometry.Vector2d vector2d, double z = 0.0)
    {
        var rhinoPoint2d = this.ToRhinoType(vector2d);

        var vector3d = new RhinoVector3d(rhinoPoint2d.X, rhinoPoint2d.Y, z);
        vector3d.Unitize();

        return vector3d;
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Geometry.Point2d"/> to a <see cref="RhinoPoint2d"/>.
    /// </summary>
    public RhinoPoint2d ToRhinoType(Autodesk.AutoCAD.Geometry.Point2d point2d)
    {
        var x = _unitSystemManager.ToRhinoLength(point2d.X);

        var y = _unitSystemManager.ToRhinoLength(point2d.Y);

        return new RhinoPoint2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Geometry.Point3d"/> to a <see cref="RhinoPoint3d"/>.
    /// </summary>
    public RhinoPoint3d ToRhinoType(Autodesk.AutoCAD.Geometry.Point3d point3d)
    {
        var x = _unitSystemManager.ToRhinoLength(point3d.X);

        var y = _unitSystemManager.ToRhinoLength(point3d.Y);

        var z = _unitSystemManager.ToRhinoLength(point3d.Z);

        return new RhinoPoint3d(x, y, z);
    }

    /// <summary>
    /// Converts a <see cref="Vector2d"/> to a unitized <see cref="RhinoVector2d"/>.
    /// </summary>
    public RhinoVector2d ToRhinoType(Autodesk.AutoCAD.Geometry.Vector2d vector2d)
    {
        var x = _unitSystemManager.ToRhinoLength(vector2d.X);

        var y = _unitSystemManager.ToRhinoLength(vector2d.Y);

        return new RhinoVector2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Geometry.Vector3d"/> to a unitized <see cref="RhinoVector3d"/>.
    /// </summary>
    public RhinoVector3d ToRhinoType(Autodesk.AutoCAD.Geometry.Vector3d vector3d)
    {
        var x = _unitSystemManager.ToRhinoLength(vector3d.X);

        var y = _unitSystemManager.ToRhinoLength(vector3d.Y);

        var z = _unitSystemManager.ToRhinoLength(vector3d.Z);

        var vector = new RhinoVector3d(x, y, z);

        vector.Unitize();

        return vector;
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.Geometry.Plane"/> to a <see cref="RhinoPlane"/>.
    /// </summary>
    public RhinoPlane ToRhinoType(Autodesk.AutoCAD.Geometry.Plane plane)
    {
        var coordinateSystem = plane.GetCoordinateSystem();

        var origin = this.ToRhinoType(plane.PointOnPlane);

        var xAxis = this.ToRhinoType(coordinateSystem.Xaxis);

        var yAxis = this.ToRhinoType(coordinateSystem.Yaxis);

        return new RhinoPlane(origin, xAxis, yAxis);
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.DatabaseServices.Line"/> to a
    /// <see cref="RhinoLineCurve"/>.
    /// </summary>
    public RhinoLineCurve ToRhinoType(Line line)
    {
        var startPoint = this.ToRhinoType(line.StartPoint);

        var endPoint = this.ToRhinoType(line.EndPoint);

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// The number of knots must be is equal to D + N - 1. Where D = degree of the curve and
    /// N = the number of control points. Some applications such as AutoCAD may have a
    /// different number  of knots (e.g D + N + 1). It is the responsibility of this class
    /// to ensure the correct number of Knots is managed. This is done by assessing the
    /// knots multiplicity. A knot is a full multiplicity knot if it is duplicated as many
    /// times as the curve has degrees. 
    /// </summary>
    private List<double> GetKnots(List<double> inputKnots, int degree,
        int numberOfControlPoints)
    {
        var targetNumberOfKnots = degree + numberOfControlPoints - 1;

        if (inputKnots.Count == targetNumberOfKnots)
            return inputKnots;

        var outputKnots = new List<double>(targetNumberOfKnots);

        var currentKnotValue = inputKnots[0];
        var currentValueCount = 1;
        outputKnots.Add(currentKnotValue);

        //Skip first as already added
        for (var index = 1; index < inputKnots.Count; index++)
        {
            var inputKnot = inputKnots[index];
            if (Math.Abs(currentKnotValue - inputKnot) > _zeroTolerance)
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
    /// Converts a <see cref="Spline"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    public RhinoNurbsCurve ToRhinoType(Spline spline)
    {
        var nurbsData = spline.NurbsData;

        var point3dCollection = nurbsData.GetControlPoints();

        var weights = nurbsData.GetWeights();

        var weightsCount = weights.Count;

        var nurbsDataKnots = nurbsData.GetKnots().ToArray().ToList();

        var knots = this.GetKnots(nurbsDataKnots, nurbsData.Degree, point3dCollection.Count);

        var rhinoNurbsCurve =
            new RhinoNurbsCurve(spline.Degree, point3dCollection.Count);

        for (var index = 0; index < point3dCollection.Count; index++)
        {

            var point3d = this.ToRhinoType(point3dCollection[index]);

            var weight = index < weightsCount ? weights[index] : 1.0;

            rhinoNurbsCurve.Points.SetPoint(index, point3d, weight);
        }

        var knotCount = knots.Count;

        var rhinoKnotCollection = rhinoNurbsCurve.Knots;

        for (var d = 0; d < knotCount; d++)
        {
            rhinoKnotCollection[d] = knots[d];
        }

        var rhinoInterval = new Rhino.Geometry.Interval(spline.StartParam, spline.EndParam);

        var trimmedNurbs = rhinoNurbsCurve.Trim(rhinoInterval);

        return trimmedNurbs as RhinoNurbsCurve ?? rhinoNurbsCurve;

    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.DatabaseServices.Ellipse"/> to a <see cref="RhinoEllipse"/>.
    /// </summary>  
    public RhinoEllipse ToRhinoType(Ellipse ellipse)
    {
        var centrePoint = this.ToRhinoType(ellipse.Center);

        var startPoint = ellipse.GetPointAtParameter(ellipse.StartParam);
        var endPoint = ellipse.GetPointAtParameter(ellipse.EndParam);

        var startPoint3d = this.ToRhinoType(startPoint);
        var endPoint3d = this.ToRhinoType(endPoint);

        var rhinoEllipse = new RhinoEllipse(centrePoint, startPoint3d, endPoint3d);

        return rhinoEllipse;
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.DatabaseServices.Arc"/> to a <see cref="RhinoArc"/>.
    /// </summary>
    public RhinoArc ToRhinoType(Arc arc)
    {
        var plane = this.ToRhinoType(arc.GetPlane());

        var radius = _unitSystemManager.ToRhinoLength(arc.Radius);

        var sweep = arc.TotalAngle;

        var rhinoArc = new RhinoArc(plane, radius, sweep)
        {
            StartAngle = arc.StartAngle,
            Angle = sweep
        };

        return rhinoArc;
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.DatabaseServices.Circle"/> to a <see cref="RhinoCircle"/>.
    /// </summary>
    public RhinoCircle ToRhinoType(Autodesk.AutoCAD.DatabaseServices.Circle circle)
    {
        var origin = this.ToRhinoType(circle.Center);

        var radius = _unitSystemManager.ToRhinoLength(circle.Radius);

        var rhinoCircle = new RhinoCircle(origin, radius);

        return rhinoCircle;
    }

    /// <summary>
    /// Converts a <see cref="Autodesk.AutoCAD.DatabaseServices.Curve"/> to a <see cref="RhinoCurve"/>.
    /// </summary>
    public RhinoCurve? ToRhinoType(Curve curve)
    {
        switch (curve)
        {
            case Line line:
                return this.ToRhinoType(line);

            case Spline spline:
                return this.ToRhinoType(spline);

            case Ellipse ellipse:
                var rhinoEllipse = this.ToRhinoType(ellipse);
                return rhinoEllipse.ToNurbsCurve();

            case Arc arc:
                var rhinoArc = this.ToRhinoType(arc);

                return rhinoArc.ToNurbsCurve();

            case Autodesk.AutoCAD.DatabaseServices.Circle circle:
                var rhinoCircle = this.ToRhinoType(circle);

                return rhinoCircle.ToNurbsCurve();

            default:
                return null;
        }
    }
}