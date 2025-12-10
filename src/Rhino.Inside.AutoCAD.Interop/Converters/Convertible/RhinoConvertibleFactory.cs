using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoConvertibleFactory"/>
public class RhinoConvertibleFactory : IRhinoConvertibleFactory
{
    /// <inheritdoc />
    public bool MakeConvertible<TRhinoType>(TRhinoType rhinoGeometry, out IRhinoConvertible? result)
        where TRhinoType : Rhino.Geometry.GeometryBase
    {
        switch (rhinoGeometry.ObjectType)
        {
            case ObjectType.Curve:
                {
                    result = new RhinoConvertibleCurve(rhinoGeometry as Curve);
                    return true;
                }
            case ObjectType.Point:
                {
                    result = new RhinoConvertiblePoint(rhinoGeometry as Point);
                    return true;
                }
            case ObjectType.Mesh:
                {
                    result = new RhinoConvertibleMesh(rhinoGeometry as Mesh);
                    return true;
                }
            case ObjectType.SubD:
                {
                    result = new RhinoConvertibleSubD(rhinoGeometry as SubD);
                    return true;
                }
            case ObjectType.Brep:
                {
                    result = new RhinoConvertibleBrep(rhinoGeometry as Brep);
                    return true;
                }
            case ObjectType.Extrusion:
                {
                    var brep = (rhinoGeometry as Extrusion).ToBrep();
                    result = new RhinoConvertibleBrep(brep);
                    return true;
                }
            case ObjectType.Surface:
                {
                    var brep = (rhinoGeometry as Surface).ToBrep();
                    result = new RhinoConvertibleBrep(brep);
                    return true;
                }
            default:
                {
                    result = null;
                    return false;
                }
        }
    }
}