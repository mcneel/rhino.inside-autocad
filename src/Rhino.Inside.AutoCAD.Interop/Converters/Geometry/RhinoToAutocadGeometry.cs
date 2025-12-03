
using CadPlane = Autodesk.AutoCAD.Geometry.Plane;
using CadPoint2d = Autodesk.AutoCAD.Geometry.Point2d;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadVector2d = Autodesk.AutoCAD.Geometry.Vector2d;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
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
    /// Converts a <see cref="RhinoPoint3d"/> to a <see cref="CadPoint2d"/> with
    /// an optional input to provide the z coordinate.
    /// </summary>
    public CadPoint2d ToAutoCadType2d(RhinoPoint3d rhinoPoint3d)
    {
        var point3d = this.ToAutoCadType(rhinoPoint3d);

        return new CadPoint2d(point3d.X, point3d.Y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector3d"/> to a <see cref="CadVector2d"/>. The
    /// vector is normalized after creation.
    /// </summary>
    public CadVector2d ToAutoCadType2d(RhinoVector3d rhinoVector3d)
    {
        var vector3d = this.ToAutoCadType(rhinoVector3d);

        var vector2d = new CadVector2d(vector3d.X, vector3d.Y);

        return vector2d.GetNormal();
    }

    /// <summary>
    /// Converts a <see cref="RhinoPoint2d"/> to a <see cref="CadPoint2d"/>.
    /// </summary>
    public CadPoint2d ToAutoCadType(RhinoPoint2d rhinoPoint2d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoPoint2d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoPoint2d.Y);

        return new CadPoint2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoPoint3d"/> to a <see cref="CadPoint3d"/>.
    /// </summary>
    public CadPoint3d ToAutoCadType(RhinoPoint3d rhinoPoint3d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.Y);

        var z = _unitSystemManager.ToAutoCadLength(rhinoPoint3d.Z);

        return new CadPoint3d(x, y, z);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector2d"/> to a unitized <see cref="CadVector2d"/>.
    /// </summary>
    public CadVector2d ToAutoCadType(RhinoVector2d rhinoVector2d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoVector2d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoVector2d.Y);

        return new CadVector2d(x, y);
    }

    /// <summary>
    /// Converts a <see cref="RhinoVector3d"/> to a unitized <see cref="CadVector3d"/>.
    /// </summary>
    public CadVector3d ToAutoCadType(RhinoVector3d rhinoVector3d)
    {
        var x = _unitSystemManager.ToAutoCadLength(rhinoVector3d.X);

        var y = _unitSystemManager.ToAutoCadLength(rhinoVector3d.Y);

        var z = _unitSystemManager.ToAutoCadLength(rhinoVector3d.Z);

        var vector = new CadVector3d(x, y, z);

        return vector.GetNormal();
    }

    /// <summary>
    /// Converts a <see cref="RhinoPlane"/> to a <see cref="CadPlane"/>.
    /// </summary>
    public CadPlane ToAutoCadType(RhinoPlane rhinoPlane)
    {
        var origin = this.ToAutoCadType(rhinoPlane.Origin);

        var xAxis = this.ToAutoCadType(rhinoPlane.XAxis);

        var yAxis = this.ToAutoCadType(rhinoPlane.YAxis);

        return new CadPlane(origin, xAxis, yAxis);
    }
}