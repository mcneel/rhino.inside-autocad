using Rhino.Geometry;
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
    public RhinoPoint3d ToRhinoType3d(Autodesk.AutoCAD.Geometry.Point2d point2d, double z = 0.0)
    {
        var rhinoPoint2d = this.ToRhinoType(point2d);

        return new RhinoPoint3d(rhinoPoint2d.X, rhinoPoint2d.Y, z);
    }

    /// <summary>
    /// Converts a <see cref="Vector2d"/> to a <see cref="RhinoVector3d"/> with
    /// an optional input to provide the z coordinate. The vector is normalized
    /// after creation.
    /// </summary>
    public RhinoVector3d ToRhinoType3d(Autodesk.AutoCAD.Geometry.Vector2d vector2d, double z = 0.0)
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
}