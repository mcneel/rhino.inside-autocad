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

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private const double _absoluteTolerance = GeometryConstants.ZeroTolerance;

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IUnitSystemManager UnitSystemManager { get; private set; }

    /// <inheritdoc />
    public IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    public RhinoInsideManager(IRhinoInstance rhinoInstance,
        IAutoCadInstance autoCadInstance, IObjectRegister objectRegister)
    {
        this.ObjectRegister = objectRegister;

        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.OnDocumentCreated += this.UpdateUnitSystem;
        autoCadInstance.OnUnitsChanged += this.UpdateUnitSystem;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.OnDocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.OnUnitChanged += this.UpdateUnitSystem;
        rhinoInstance.OnObjectModifiedOrAppended += this.OnRhinoObjectModifiedOrAppended;
        rhinoInstance.OnObjectRemoved += this.OnRhinoObjectRemoved;

        var unitsSystemManager = new UnitSystemManager(_defaultUnitSystem, _defaultUnitSystem);

        GeometryConverter.Initialize(unitsSystemManager);

        _geometryConverter = GeometryConverter.Instance!;

        this.UnitSystemManager = unitsSystemManager;

    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is removed.
    /// </summary>
    private void OnRhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        if (this.ObjectRegister.TryGetObject(rhinoObject, out var oldEntities))
        {
            var rhinoPreview = this.AutoCadInstance.RhinoObjectPreviewer;

            rhinoPreview.RemoveEntities(oldEntities);

            this.ObjectRegister.RemoveObject(rhinoObject);
        }

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is modified or appended.
    /// </summary>
    private void OnRhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        var rhinoPreview = this.AutoCadInstance.RhinoObjectPreviewer;

        if (this.ObjectRegister.TryGetObject(rhinoObject, out var oldEntities))
        {
            rhinoPreview.RemoveEntities(oldEntities);
        }

        if (this.TryConvert(rhinoObject, out var newEntities))
        {
            this.ObjectRegister.RegisterObject(rhinoObject, newEntities);

            rhinoPreview.AddEntities(newEntities);
        }

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    /// <summary>
    /// Tries to convert a Rhino object to AutoCAD entities.
    /// </summary>
    private bool TryConvert(RhinoObject rhinoObject, out List<IEntity> entities)
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

    private void UpdateUnitSystem(object sender, EventArgs e)
    {
        var autoCadUnits = this.AutoCadInstance.ActiveDocument?.UnitSystem ?? _defaultUnitSystem;
        var rhinoUnits = this.RhinoInstance.ActiveDoc?.ModelUnitSystem ?? _defaultUnitSystem;

        if (this.UnitSystemManager.AutoCadUnits != autoCadUnits ||
            this.UnitSystemManager.RhinoUnits != rhinoUnits)
        {
            var unitsSystemManager = new UnitSystemManager(autoCadUnits, rhinoUnits);
            this.UnitSystemManager = unitsSystemManager;

            GeometryConverter.Initialize(unitsSystemManager);

            _geometryConverter = GeometryConverter.Instance!;
        }
    }
}
