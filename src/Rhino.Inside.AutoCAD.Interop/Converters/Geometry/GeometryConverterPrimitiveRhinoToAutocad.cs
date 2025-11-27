using Autodesk.AutoCAD.Geometry;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoVector2d = Rhino.Geometry.Vector2d;
using RhinoVector3d = Rhino.Geometry.Vector3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// This class handles conversion of primitive geometry types between Rhino and AutoCAD.
/// </summary>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts a <see cref="RhinoPoint3d"/> to a <see cref="Autodesk.AutoCAD.Geometry.Point2d"/> with
    /// an optional input to provide the z coordinate.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Point2d ConvertTo2d(RhinoPoint3d rhinoPoint3d)
    {
        var point3d = this.ToRhinoType(rhinoPoint3d);

        return new Autodesk.AutoCAD.Geometry.Point2d(point3d.X, point3d.Y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector3d"/> to a <see cref="Vector2d"/>. The
    /// vector is normalized after creation.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Vector2d ConvertTo2d(RhinoVector3d rhinoVector3d)
    {
        var vector3d = this.ToRhinoType(rhinoVector3d);

        var vector2d = new Autodesk.AutoCAD.Geometry.Vector2d(vector3d.X, vector3d.Y);

        return vector2d.GetNormal();
    }

    /// <summary>
    /// Converts a <see cref="RhinoPoint2d"/> to a <see cref="Autodesk.AutoCAD.Geometry.Point2d"/>.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Point2d ToRhinoType(RhinoPoint2d rhinoPoint2d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoPoint2d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoPoint2d.Y);

        return new Autodesk.AutoCAD.Geometry.Point2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoPoint3d"/> to a <see cref="Autodesk.AutoCAD.Geometry.Point3d"/>.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Point3d ToRhinoType(RhinoPoint3d rhinoPoint3d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.Y);

        var z = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.Z);

        return new Autodesk.AutoCAD.Geometry.Point3d(x, y, z);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector2d"/> to a unitized <see cref="Vector2d"/>.
    /// </summary>
    public Vector2d ToRhinoType(RhinoVector2d rhinoVector2d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoVector2d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoVector2d.Y);

        return new Vector2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector3d"/> to a unitized <see cref="Autodesk.AutoCAD.Geometry.Vector3d"/>.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Vector3d ToRhinoType(RhinoVector3d rhinoVector3d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoVector3d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoVector3d.Y);

        var z = _unitSystemManager.ToAutoCadLength(rhinoVector3d.Z);

        var vector = new Autodesk.AutoCAD.Geometry.Vector3d(x, y, z);

        return vector.GetNormal();
    }

    /// <summary>
    /// Converts a <see cref="RhinoPlane"/> to a <see cref="Autodesk.AutoCAD.Geometry.Plane"/>.
    /// </summary>
    public Autodesk.AutoCAD.Geometry.Plane ToRhinoType(RhinoPlane rhinoPlane)
    {
        var origin = this.ToRhinoType(rhinoPlane.Origin);

        var xAxis = this.ToRhinoType(rhinoPlane.XAxis);

        var yAxis = this.ToRhinoType(rhinoPlane.YAxis);

        return new Autodesk.AutoCAD.Geometry.Plane(origin, xAxis, yAxis);
    }
}