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
                    result = new RhinoConvertibleExtrusion(rhinoGeometry as Extrusion);
                    return true;
                }
            case ObjectType.Surface:
                {
                    result = new RhinoConvertibleSurface(rhinoGeometry as Surface);
                    return true;
                }
            case ObjectType.Hatch:
                {
                    result = new RhinoConvertibleHatch(rhinoGeometry as Hatch);
                    return true;
                }
            case ObjectType.Annotation:
                {
                    var annotation = rhinoGeometry as AnnotationBase;

                    switch (annotation)
                    {
                        case TextEntity textEntity:
                            result = new RhinoConvertibleText(textEntity);
                            return true;
                        case Dimension dimension:
                            result = new RhinoConvertibleDimension(dimension);
                            return true;
                        case Leader leader:
                            result = new RhinoConvertibleLeader(leader);
                            return true;
                        default:
                            result = null;
                            return false;
                    }
                }
            default:
                {
                    result = null;
                    return false;
                }
        }
    }
}