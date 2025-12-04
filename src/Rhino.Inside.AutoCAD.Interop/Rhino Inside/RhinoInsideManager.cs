using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private readonly IRhinoObjectConverter _rhinoObjectConvert;
    private readonly IGrasshopperGeometryExtractor _grasshopperGeometryExtractor;

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IGrasshopperInstance GrasshopperInstance { get; }

    /// <inheritdoc />
    public IUnitSystemManager UnitSystemManager { get; private set; }

    /// <inheritdoc />
    public IRhinoObjectPreviewServer RhinoPreviewServer { get; }

    /// <inheritdoc />
    public IGrasshopperObjectPreviewServer GrasshopperPreviewServer { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    public RhinoInsideManager(IRhinoInstance rhinoInstance, IGrasshopperInstance grasshopperInstance,
        IAutoCadInstance autoCadInstance)
    {

        this.RhinoPreviewServer = new RhinoObjectPreviewServer();

        this.GrasshopperPreviewServer = new GrasshopperObjectPreviewServer();

        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.OnDocumentCreated += this.UpdateUnitSystem;
        autoCadInstance.OnUnitsChanged += this.UpdateUnitSystem;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.OnDocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.OnUnitChanged += this.UpdateUnitSystem;
        rhinoInstance.OnObjectModifiedOrAppended += this.OnRhinoObjectModifiedOrAppended;
        rhinoInstance.OnObjectRemoved += this.OnRhinoObjectRemoved;

        this.GrasshopperInstance = grasshopperInstance;
        grasshopperInstance.OnPreviewExpired += this.UpdateGrasshopperPreview;
        grasshopperInstance.OnObjectRemoved += this.OnGrasshopperObjectRemoved;

        var unitsSystemManager = new UnitSystemManager(_defaultUnitSystem, _defaultUnitSystem);

        GeometryConverter.Initialize(unitsSystemManager);
        _rhinoObjectConvert = new RhinoObjectConverter(GeometryConverter.Instance!);

        this.UnitSystemManager = unitsSystemManager;
        _grasshopperGeometryExtractor = new GrasshopperGeometryExtractor();
    }

    /// <summary>
    /// Removes the preview of a Grasshopper object from the <see cref="GrasshopperPreviewServer"/>
    /// when it is removed from the Grasshopper document.
    /// </summary>
    private void OnGrasshopperObjectRemoved(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Grasshopper object's preview expires.
    /// </summary>
    private void UpdateGrasshopperPreview(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        var ghDocumentObject = e.GrasshopperObject;

        var instanceGuid = ghDocumentObject.InstanceGuid;

        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);

        var previewGeometryData = _grasshopperGeometryExtractor.ExtractPreviewGeometry(ghDocumentObject);

        var convertedWireframeEntities = previewGeometryData.GetWireframeEntities();

        var convertedShadedEntities = previewGeometryData.GetShadedEntities();

        this.GrasshopperPreviewServer.AddObject(instanceGuid, convertedWireframeEntities, convertedShadedEntities);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();

    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is removed.
    /// </summary>
    private void OnRhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is modified or appended.
    /// </summary>
    private void OnRhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        if (_rhinoObjectConvert.TryConvert(rhinoObject, out var newEntities))
        {
            this.RhinoPreviewServer.AddObject(rhinoObject.Id, newEntities);
        }

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
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

        }
    }
}