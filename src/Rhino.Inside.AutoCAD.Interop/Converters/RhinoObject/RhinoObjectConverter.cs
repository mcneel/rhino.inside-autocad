using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoSubD = Rhino.Geometry.SubD;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A Service to convert <see cref="RhinoObject"/> to Autocad <see cref="IEntity"/>.
/// </summary>
public interface IRhinoObjectConverter
{
    /// <summary>
    /// Tries to convert a Rhino object to AutoCAD entities.
    /// </summary>
    bool TryConvert(RhinoObject rhinoObject, out List<IEntity> entities);
}

/// <inheritdoc cref="IRhinoObjectConverter"/>
public class RhinoObjectConverter : IRhinoObjectConverter
{
    private readonly GeometryConverter _geometryConverter;
    private const double _absoluteTolerance = GeometryConstants.ZeroTolerance;

    /// <summary>
    /// Constructs a singleton instance of the <see cref="RhinoObjectConverter"/>.
    /// The <see cref="GeometryConverter"/> instance is required to perform geometry
    /// conversions it is passed as a parameter so that unit systems are always the latest
    /// and forcing the <see cref="GeometryConverter"/> to be initialized first.
    /// </summary>
    public RhinoObjectConverter(GeometryConverter geometryConverter)
    {
        _geometryConverter = geometryConverter;
    }

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
                        var entity = new AutocadEntityWrapper(curve);

                        entities.Add(entity);
                    }

                    return true;
                }
            case ObjectType.Point:
                {
                    var rhinoPoint = geometry as RhinoPoint;

                    var point3d = _geometryConverter.ToAutoCadType(rhinoPoint!.Location);

                    var acPoint = new Autodesk.AutoCAD.DatabaseServices.DBPoint(point3d);

                    var entity = new AutocadEntityWrapper(acPoint);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.Mesh:
                {
                    var rhinoMesh = geometry as RhinoMesh;

                    var cadMesh = _geometryConverter.ToAutoCadType(rhinoMesh!);

                    var entity = new AutocadEntityWrapper(cadMesh);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.SubD:
                {
                    var rhinoMesh = geometry as RhinoSubD;

                    var cadMesh = _geometryConverter.ToAutoCadType(rhinoMesh!);

                    var entity = new AutocadEntityWrapper(cadMesh);

                    entities.Add(entity);
                    return true;
                }
            case ObjectType.Brep:
            case ObjectType.Extrusion:
            case ObjectType.Surface:
                {
                    var renderMeshes = rhinoObject.GetMeshes(MeshType.Preview);

                    if (renderMeshes == null || renderMeshes.Length == 0)
                    {
                        rhinoObject.CreateMeshes(MeshType.Preview, MeshingParameters.Default, true);
                        renderMeshes = rhinoObject.GetMeshes(MeshType.Preview);
                    }

                    foreach (var renderMesh in renderMeshes)
                    {
                        var cadMesh = _geometryConverter.ToAutoCadType(renderMesh);

                        var entity = new AutocadEntityWrapper(cadMesh);

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
                            var entity = new AutocadEntityWrapper(solid);

                            entities.Add(entity);
                        }
                    }
                    return true;
                }
            default: return false;

        }
    }
}