using CadPoint2d = Autodesk.AutoCAD.Geometry.Point2d;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadVector2d = Autodesk.AutoCAD.Geometry.Vector2d;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
using CadPlane = Autodesk.AutoCAD.Geometry.Plane;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPoint3f = Rhino.Geometry.Point3f;
using RhinoVector2d = Rhino.Geometry.Vector2d;
using RhinoVector3d = Rhino.Geometry.Vector3d;
using RhinoPlane = Rhino.Geometry.Plane;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Rhino geometry types to AutoCAD geometry types.
/// </summary>
public static class RhinoGeometryExtensions
{
    /// <summary>
    /// Converts a Rhino 2D point to an AutoCAD 2D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The Rhino 2D point to convert.</param>
    /// <returns>An AutoCAD 2D point with coordinates scaled to AutoCAD units.</returns>
    public static CadPoint2d ToAutocadPoint2d(this RhinoPoint2d point)
    {
        return new CadPoint2d(
            UnitConverter.ToAutoCadLength(point.X),
            UnitConverter.ToAutoCadLength(point.Y));
    }

    /// <summary>
    /// Converts a Rhino 3D point to an AutoCAD 3D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The Rhino 3D point to convert.</param>
    /// <returns>An AutoCAD 3D point with coordinates scaled to AutoCAD units.</returns>
    public static CadPoint3d ToAutocadPoint3d(this RhinoPoint3d point)
    {
        return new CadPoint3d(
            UnitConverter.ToAutoCadLength(point.X),
            UnitConverter.ToAutoCadLength(point.Y),
            UnitConverter.ToAutoCadLength(point.Z));
    }

    /// <summary>
    /// Converts a Rhino 3D point to an AutoCAD 2D point, applying unit conversion.
    /// Discards the Z coordinate.
    /// </summary>
    /// <param name="point">The Rhino 3D point to convert.</param>
    /// <returns>An AutoCAD 2D point with X/Y coordinates scaled to AutoCAD units.</returns>
    public static CadPoint2d ToAutocadPoint2d(this RhinoPoint3d point)
    {
        return new CadPoint2d(
            UnitConverter.ToAutoCadLength(point.X),
            UnitConverter.ToAutoCadLength(point.Y));
    }

    /// <summary>
    /// Converts a Rhino 3D float point (mesh vertex) to an AutoCAD 3D point, applying unit conversion.
    /// </summary>
    /// <param name="point">The Rhino 3D float point to convert.</param>
    /// <returns>An AutoCAD 3D point with coordinates scaled to AutoCAD units.</returns>
    public static CadPoint3d ToAutocadPoint3d(this RhinoPoint3f point)
    {
        return new CadPoint3d(
            UnitConverter.ToAutoCadLength(point.X),
            UnitConverter.ToAutoCadLength(point.Y),
            UnitConverter.ToAutoCadLength(point.Z));
    }

    /// <summary>
    /// Converts a Rhino 2D vector to an AutoCAD 2D vector, applying unit conversion.
    /// </summary>
    /// <param name="vector">The Rhino 2D vector to convert.</param>
    /// <returns>An AutoCAD 2D vector with components scaled to AutoCAD units.</returns>
    public static CadVector2d ToAutocadVector2d(this RhinoVector2d vector)
    {
        return new CadVector2d(
            UnitConverter.ToAutoCadLength(vector.X),
            UnitConverter.ToAutoCadLength(vector.Y));
    }

    /// <summary>
    /// Converts a Rhino 3D vector to an AutoCAD 3D vector, applying unit conversion.
    /// The vector is normalized after conversion.
    /// </summary>
    /// <param name="vector">The Rhino 3D vector to convert.</param>
    /// <returns>A normalized AutoCAD 3D vector with components scaled to AutoCAD units.</returns>
    public static CadVector3d ToAutocadVector3d(this RhinoVector3d vector)
    {
        var cadVector = new CadVector3d(
            UnitConverter.ToAutoCadLength(vector.X),
            UnitConverter.ToAutoCadLength(vector.Y),
            UnitConverter.ToAutoCadLength(vector.Z));

        return cadVector.GetNormal();
    }

    /// <summary>
    /// Converts a Rhino 3D vector to an AutoCAD 2D vector, applying unit conversion.
    /// Discards the Z component. The vector is normalized after conversion.
    /// </summary>
    /// <param name="vector">The Rhino 3D vector to convert.</param>
    /// <returns>A normalized AutoCAD 2D vector with X/Y components scaled to AutoCAD units.</returns>
    public static CadVector2d ToAutocadVector2d(this RhinoVector3d vector)
    {
        var cadVector = new CadVector2d(
            UnitConverter.ToAutoCadLength(vector.X),
            UnitConverter.ToAutoCadLength(vector.Y));

        return cadVector.GetNormal();
    }

    /// <summary>
    /// Converts a Rhino plane to an AutoCAD plane, applying unit conversion to the origin.
    /// </summary>
    /// <param name="plane">The Rhino plane to convert.</param>
    /// <returns>An AutoCAD plane with origin scaled to AutoCAD units.</returns>
    public static CadPlane ToAutocadPlane(this RhinoPlane plane)
    {
        return new CadPlane(
            plane.Origin.ToAutocadPoint3d(),
            plane.XAxis.ToAutocadVector3d(),
            plane.YAxis.ToAutocadVector3d());
    }
}
