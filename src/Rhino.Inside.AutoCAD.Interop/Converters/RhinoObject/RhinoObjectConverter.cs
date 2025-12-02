using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoExtrusion = Rhino.Geometry.Extrusion;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoSubD = Rhino.Geometry.SubD;
using RhinoSurface = Rhino.Geometry.Surface;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A Service to convert <see cref="RhinoObject"/> to Autocad <see cref="IEntity"/>.
/// </summary>
public class RhinoObjectConverter
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private const double _absoluteTolerance = GeometryConstants.ZeroTolerance;

    /// <summary>
    /// Returns the <see cref="InternalColorConverter"/> singleton.
    /// </summary>
    public static RhinoObjectConverter Instance { get; }

    /// <summary>
    /// Static constructor that initializes the <see cref="RhinoObjectConverter"/> singleton.
    /// </summary>
    static RhinoObjectConverter()
    {
        RhinoObjectConverter.Instance = new RhinoObjectConverter();
    }

    /// <summary>
    /// Constructs a singleton instance of the <see cref="RhinoObjectConverter"/>.
    /// </summary>
    private RhinoObjectConverter() { }

    /// <summary>
    /// Tries to convert a Rhino object to AutoCAD entities.
    /// </summary>
    public bool TryConvert(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        var geometry = rhinoObject.Geometry;

        entities = new List<IEntity>();

        switch (geometry.ObjectType)
        {
            case ObjectType.Curve:
                {
                    var rhinoCurve = geometry as RhinoCurve;

                    var curves = _geometryConverter.ToAutoCadType(rhinoCurve!);

                    foreach (var curve in curves)
                    {
                        var entity = new Entity(curve);

                        entities.Add(entity);
                    }

                    return true;
                }
            case ObjectType.Point:
                {
                    var rhinoPoint = geometry as RhinoPoint;

                    var point3d = _geometryConverter.ToAutoCadType(rhinoPoint!.Location);

                    var acPoint = new Autodesk.AutoCAD.DatabaseServices.DBPoint(point3d);

                    var entity = new Entity(acPoint);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.Mesh:
                {
                    var rhinoMesh = geometry as RhinoMesh;

                    var cadMesh = _geometryConverter.ToAutoCadType(rhinoMesh!);

                    var entity = new Entity(cadMesh);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.SubD:
                {
                    var rhinoMesh = geometry as RhinoSubD;

                    var cadMesh = _geometryConverter.ToAutoCadType(rhinoMesh!);

                    var entity = new Entity(cadMesh);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.Brep:
                {
                    var brep = geometry as RhinoBrep;

                    var cadSolid = _geometryConverter.ToAutoCadType(brep!);

                    foreach (var solid in cadSolid)
                    {
                        var entity = new Entity(solid);

                        entities.Add(entity);
                    }

                    return true;
                }
            case ObjectType.Extrusion:
                {
                    var extrusion = geometry as RhinoExtrusion;

                    var cadSolid = _geometryConverter.ToAutoCadType(extrusion!.ToBrep());

                    foreach (var solid in cadSolid)
                    {
                        var entity = new Entity(solid);

                        entities.Add(entity);
                    }

                    return true;
                }

            case ObjectType.Hatch:
                {
                    var rhinoHatch = geometry as RhinoHatch;

                    var borders = rhinoHatch!.Get3dCurves(false);

                    var breps = RhinoBrep.CreatePlanarBreps(borders, _absoluteTolerance);

                    foreach (var brep in breps)
                    {
                        var cadHatch = _geometryConverter.ToAutoCadType(brep);

                        foreach (var solid in cadHatch)
                        {
                            var entity = new Entity(solid);

                            entities.Add(entity);
                        }
                    }
                    return true;
                }
            case ObjectType.Surface:
                {
                    var rhinoSurface = geometry as RhinoSurface;

                    var cadSurface = _geometryConverter.ToAutoCadType(rhinoSurface!.ToBrep());

                    foreach (var solid in cadSurface)
                    {
                        var entity = new Entity(solid);

                        entities.Add(entity);
                    }

                    return true;
                }

            default: return false;

        }
    }
}