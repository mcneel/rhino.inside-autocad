using CadPoint2d = Autodesk.AutoCAD.Geometry.Point2d;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadVector2d = Autodesk.AutoCAD.Geometry.Vector2d;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
using CadPlane = Autodesk.AutoCAD.Geometry.Plane;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoVector2d = Rhino.Geometry.Vector2d;
using RhinoVector3d = Rhino.Geometry.Vector3d;
using RhinoPlane = Rhino.Geometry.Plane;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting AutoCAD geometry types to Rhino geometry types.
/// </summary>
public static class AutocadGeometryExtensions
{
    /// <summary>
    /// Converts an AutoCAD 2D point to a Rhino 2D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The AutoCAD 2D point to convert.</param>
    /// <returns>A Rhino 2D point with coordinates scaled to Rhino units.</returns>
    public static RhinoPoint2d ToRhinoPoint2d(this CadPoint2d point)
    {
        return new RhinoPoint2d(
            UnitConverter.ToRhinoLength(point.X),
            UnitConverter.ToRhinoLength(point.Y));
    }

    /// <summary>
    /// Converts an AutoCAD 3D point to a Rhino 3D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The AutoCAD 3D point to convert.</param>
    /// <returns>A Rhino 3D point with coordinates scaled to Rhino units.</returns>
    public static RhinoPoint3d ToRhinoPoint3d(this CadPoint3d point)
    {
        return new RhinoPoint3d(
            UnitConverter.ToRhinoLength(point.X),
            UnitConverter.ToRhinoLength(point.Y),
            UnitConverter.ToRhinoLength(point.Z));
    }

    /// <summary>
    /// Converts an AutoCAD 2D point to a Rhino 3D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The AutoCAD 2D point to convert.</param>
    /// <param name="z">The Z coordinate value in Rhino units. Defaults to 0.</param>
    /// <returns>A Rhino 3D point with X/Y coordinates scaled to Rhino units.</returns>
    public static RhinoPoint3d ToRhinoPoint3d(this CadPoint2d point, double z = 0.0)
    {
        return new RhinoPoint3d(
            UnitConverter.ToRhinoLength(point.X),
            UnitConverter.ToRhinoLength(point.Y),
            z);
    }

    /// <summary>
    /// Converts an AutoCAD 2D vector to a Rhino 2D vector, applying unit conversion.
    /// </summary>
    /// <param name="vector">The AutoCAD 2D vector to convert.</param>
    /// <returns>A Rhino 2D vector with components scaled to Rhino units.</returns>
    public static RhinoVector2d ToRhinoVector2d(this CadVector2d vector)
    {
        return new RhinoVector2d(
            UnitConverter.ToRhinoLength(vector.X),
            UnitConverter.ToRhinoLength(vector.Y));
    }

    /// <summary>
    /// Converts an AutoCAD 3D vector to a Rhino 3D vector, applying unit conversion.
    /// The vector is unitized after conversion.
    /// </summary>
    /// <param name="vector">The AutoCAD 3D vector to convert.</param>
    /// <returns>A unitized Rhino 3D vector with components scaled to Rhino units.</returns>
    public static RhinoVector3d ToRhinoVector3d(this CadVector3d vector)
    {
        var rhinoVector = new RhinoVector3d(
            UnitConverter.ToRhinoLength(vector.X),
            UnitConverter.ToRhinoLength(vector.Y),
            UnitConverter.ToRhinoLength(vector.Z));

        rhinoVector.Unitize();

        return rhinoVector;
    }

    /// <summary>
    /// Converts an AutoCAD 2D vector to a Rhino 3D vector, applying unit conversion.
    /// The vector is unitized after conversion.
    /// </summary>
    /// <param name="vector">The AutoCAD 2D vector to convert.</param>
    /// <param name="z">The Z component value in Rhino units. Defaults to 0.</param>
    /// <returns>A unitized Rhino 3D vector with X/Y components scaled to Rhino units.</returns>
    public static RhinoVector3d ToRhinoVector3d(this CadVector2d vector, double z = 0.0)
    {
        var rhinoVector2d = vector.ToRhinoVector2d();

        var vector3d = new RhinoVector3d(rhinoVector2d.X, rhinoVector2d.Y, z);
        vector3d.Unitize();

        return vector3d;
    }

    /// <summary>
    /// Converts an AutoCAD plane to a Rhino plane, applying unit conversion to the origin.
    /// </summary>
    /// <param name="plane">The AutoCAD plane to convert.</param>
    /// <returns>A Rhino plane with origin scaled to Rhino units.</returns>
    public static RhinoPlane ToRhinoPlane(this CadPlane plane)
    {
        var coordinateSystem = plane.GetCoordinateSystem();

        var origin = plane.PointOnPlane.ToRhinoPoint3d();
        var xAxis = coordinateSystem.Xaxis.ToRhinoVector3d();
        var yAxis = coordinateSystem.Yaxis.ToRhinoVector3d();

        return new RhinoPlane(origin, xAxis, yAxis);
    }
}
