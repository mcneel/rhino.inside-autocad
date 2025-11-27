using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly IObjectRegister _objectRegister;
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IUnitSystemManager UnitSystemManager { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    public RhinoInsideManager(IRhinoInstance rhinoInstance,
        IAutoCadInstance autoCadInstance, IObjectRegister objectRegister)
    {
        _objectRegister = objectRegister;
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

    private void OnRhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        if (_objectRegister.TryGetObjectId(rhinoObject, out var oldEntities))
        {
            this.AutoCadInstance.TransientManager!.RemoveEntities(oldEntities);
        }
    }

    private void OnRhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        if (_objectRegister.TryGetObjectId(rhinoObject, out var oldEntities))
        {
            this.AutoCadInstance.TransientManager!.RemoveEntities(oldEntities);
        }

        if (this.TryConvert(rhinoObject, out var newEntities))
        {
            _objectRegister.RegisterObjectId(rhinoObject, newEntities);
            this.AutoCadInstance.TransientManager!.AddEntities(newEntities);
        }
    }

    private bool TryConvert(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        var geometry = rhinoObject.Geometry;

        entities = new List<IEntity>();

        switch (geometry.ObjectType)
        {
            case ObjectType.Curve:
                {
                    var rhinoCurve = geometry as RhinoCurve;

                    var curves = _geometryConverter.ToAutoCadType(rhinoCurve);

                    foreach (var curve in curves)
                    {
                        var entity = new Entity(curve);
                        entities.Add(entity);
                    }

                    return true;
                }
            default: return false;

        }
    }

    private void UpdateUnitSystem(object sender, EventArgs e)
    {
        var autoCadUnits = this.AutoCadInstance.Document?.UnitSystem ?? _defaultUnitSystem;
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
