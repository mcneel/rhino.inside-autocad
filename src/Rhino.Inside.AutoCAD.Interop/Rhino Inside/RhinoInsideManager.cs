using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private readonly IRhinoObjectConverter _rhinoObjectConvert;
    private readonly IGrasshopperGeometryExtractor _grasshopperGeometryExtractor;
    private readonly IGrasshopperChangeResponder _grasshopperChangeResponder;
    private readonly IRhinoConvertibleFactory _rhinoConvertibleFactory;

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

        var previewGeometryConverter = new PreviewGeometryConverter(autoCadInstance);

        _rhinoConvertibleFactory = new RhinoConvertibleFactory();

        var rhinoPreviewSettings = new GeometryPreviewSettings(128,
            "Rhino.Inside.AutoCAD.Preview.Rhino.Material", 4);

        this.RhinoPreviewServer = new RhinoObjectPreviewServer(rhinoPreviewSettings, previewGeometryConverter);

        var grasshopperPreviewSettings = new GeometryPreviewSettings(128,
            "Rhino.Inside.AutoCAD.Preview.Grasshopper.Material", 1);

        this.GrasshopperPreviewServer = new GrasshopperObjectPreviewServer(
            grasshopperPreviewSettings, previewGeometryConverter, _rhinoConvertibleFactory);

        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.DocumentCreated += this.AutocadDocumentSwitched;
        autoCadInstance.UnitsChanged += this.UpdateUnitSystem;
        autoCadInstance.DocumentChanged += this.AutocadDocumentChange;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.DocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.UnitsChanged += this.UpdateUnitSystem;
        rhinoInstance.ObjectModifiedOrAppended += this.RhinoObjectModifiedOrAppended;
        rhinoInstance.ObjectRemoved += this.RhinoObjectRemoved;

        this.GrasshopperInstance = grasshopperInstance;
        grasshopperInstance.OnPreviewExpired += this.UpdateGrasshopperPreview;
        grasshopperInstance.OnObjectRemoved += this.OnGrasshopperObjectRemoved;

        var unitsSystemManager = new UnitSystemManager(_defaultUnitSystem, _defaultUnitSystem);

        GeometryConverter.Initialize(unitsSystemManager);
        _rhinoObjectConvert = new RhinoObjectConverter(GeometryConverter.Instance!);

        this.UnitSystemManager = unitsSystemManager;
        _grasshopperGeometryExtractor = new GrasshopperGeometryExtractor(_rhinoConvertibleFactory);
        _grasshopperChangeResponder = new GrasshopperChangeResponder();
    }

    /// <summary>
    /// Handles AutoCAD document switching and creates preview materials in both Rhino and Grasshopper
    /// preview servers.
    /// </summary>
    private void AutocadDocumentSwitched(object sender, EventArgs e)
    {
        this.UpdateUnitSystem(sender, e);

        var document = this.AutoCadInstance.ActiveDocument;

        if (document == null) return;

        this.RhinoPreviewServer.Settings.CreateMaterial(document);

        this.GrasshopperPreviewServer.Settings.CreateMaterial(document);
    }

    /// <summary>
    /// Handles AutoCAD document changes and responds to them in Grasshopper.
    /// </summary>
    private void AutocadDocumentChange(object sender, IAutocadDocumentChangeEventArgs e)
    {
        _grasshopperChangeResponder.Respond(e.Change);
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

        this.GrasshopperPreviewServer.AddObject(instanceGuid, previewGeometryData);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();

    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is removed.
    /// </summary>
    private void RhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is modified or appended.
    /// </summary>
    private void RhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        if (_rhinoConvertibleFactory.MakeConvertible(rhinoObject.Geometry, out var rhinoConvertible))
        {
            var newSet = new RhinoConvertibleSet { rhinoConvertible };

            this.RhinoPreviewServer.AddObject(rhinoObject.Id, newSet);
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
