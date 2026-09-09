using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.State;
using UnitConverterClass = Rhino.Inside.AutoCAD.Interop.UnitConverter;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private readonly IGrasshopperGeometryExtractor _grasshopperGeometryExtractor;
    private readonly IGrasshopperChangeResponder _grasshopperChangeResponder;
    private readonly IRhinoConvertibleFactory _rhinoConvertibleFactory;
    private readonly IPreviewMaterialScheduler _previewMaterialScheduler;

    /// <summary>
    /// Contains failures raised inside the handlers below.
    /// </summary>
    /// <remarks>
    /// Every handler in this class is reached from a Rhino, Grasshopper or AutoCAD reactor,
    /// which is native code. An exception escaping one has no managed frame above it to be
    /// caught by and terminates the host, so each handler body is run through this.
    /// </remarks>
    private readonly IAutocadGuard _autocadGuard = new AutocadGuard();

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IGrasshopperInstance GrasshopperInstance { get; }

    /// <inheritdoc />
    public IUnitConverter UnitConverter { get; private set; }

    /// <inheritdoc />
    public IRhinoObjectPreviewServer RhinoPreviewServer { get; }

    /// <inheritdoc />
    public IGrasshopperObjectPreviewServer GrasshopperPreviewServer { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    /// <param name="rhinoInstance">The Rhino instance to manage.</param>
    /// <param name="grasshopperInstance">The Grasshopper instance to manage.</param>
    /// <param name="autoCadInstance">The AutoCAD instance to manage.</param>
    /// <param name="userSettings">
    /// The user settings the preview colors are read from. Only read here: later changes
    /// reach the previews through <see cref="UpdatePreviewColors"/>.
    /// </param>
    public RhinoInsideManager(IRhinoInstance rhinoInstance, IGrasshopperInstance grasshopperInstance,
        IAutoCadInstance autoCadInstance, IUserSettings userSettings)
    {
        var previewGeometryConverter = new PreviewGeometryConverter(autoCadInstance);

        _rhinoConvertibleFactory = new RhinoConvertibleFactory();

        var selectedPreviewSettings = new GeometryPreviewSettings(128,
            "Rhino.Inside.AutoCAD.Preview.Selected.Material",
            userSettings.SelectedPreviewColorIndex);

        var rhinoPreviewSettings = new GeometryPreviewSettings(128,
            "Rhino.Inside.AutoCAD.Preview.Rhino.Material",
            userSettings.RhinoPreviewColorIndex);

        this.RhinoPreviewServer = new RhinoObjectPreviewServer(rhinoPreviewSettings, selectedPreviewSettings, previewGeometryConverter);

        var grasshopperPreviewSettings = new GeometryPreviewSettings(128,
            "Rhino.Inside.AutoCAD.Preview.Grasshopper.Material",
            userSettings.GrasshopperPreviewColorIndex);

        this.GrasshopperPreviewServer = new GrasshopperObjectPreviewServer(
            grasshopperPreviewSettings, selectedPreviewSettings, previewGeometryConverter);

        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.DocumentActivated += this.AutocadDocumentSwitched;
        autoCadInstance.UnitsChanged += this.UpdateUnitSystem;
        autoCadInstance.DocumentChanged += this.AutocadDocumentChange;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.DocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.UnitsChanged += this.UpdateUnitSystem;
        rhinoInstance.ObjectModifiedOrAppended += this.RhinoObjectModifiedOrAppended;
        rhinoInstance.ObjectRemoved += this.RhinoObjectRemoved;
        rhinoInstance.DeselectAll += this.DeselectAllRhinoPreview;

        this.GrasshopperInstance = grasshopperInstance;
        grasshopperInstance.PreviewExpired += this.OnUpdateGrasshopperPreview;
        grasshopperInstance.ObjectRemoved += this.OnGrasshopperObjectRemoved;
        grasshopperInstance.ComponentSelectionChanged +=
            this.OnGrasshopperSelectionChanged;

        UnitConverterClass.Initialize(_defaultUnitSystem, _defaultUnitSystem);

        this.UnitConverter = UnitConverterClass.Instance!;
        _grasshopperGeometryExtractor = new GrasshopperGeometryExtractor(_rhinoConvertibleFactory);
        _grasshopperChangeResponder = new GrasshopperChangeResponder();
        _previewMaterialScheduler = new PreviewMaterialScheduler(this.RefreshPreviewAppearance);
    }

    /// <summary>
    /// Restyles every drawn preview so it picks up a material created after it was drawn.
    /// </summary>
    private void RefreshPreviewAppearance()
    {
        this.RhinoPreviewServer.RefreshAppearance();

        this.GrasshopperPreviewServer.RefreshAppearance();
    }

    /// <summary>
    /// Requests the preview materials for the given document, creating any that are missing
    /// once AutoCAD is idle.
    /// </summary>
    /// <remarks>
    /// The two servers share one settings instance for the selected state, so the selected
    /// settings are taken from the Rhino server only. All three are requested together so
    /// that none is left to be created on first use, which happens under a reactor.
    /// </remarks>
    private void EnsurePreviewMaterials(IAutocadDocument document)
    {
        _previewMaterialScheduler.EnsureCreated(document,
            this.RhinoPreviewServer.UnSelectedSettings,
            this.GrasshopperPreviewServer.UnSelectedSettings,
            this.RhinoPreviewServer.SelectedSettings);
    }

    /// <inheritdoc />
    public void EnsurePreviewMaterials()
    {
        var document = this.AutoCadInstance.ActiveDocument;

        if (document == null) return;

        this.EnsurePreviewMaterials(document);
    }

    /// <summary>
    /// Handles AutoCAD document switching, syncing units and requesting the preview materials
    /// for the newly activated document.
    /// </summary>
    /// <remarks>
    /// Raised from AutoCAD's own document-activation reactor, so the materials are only
    /// requested here, never created: the creation is deferred to the idle loop by
    /// <see cref="IPreviewMaterialScheduler"/>.
    /// </remarks>
    private void AutocadDocumentSwitched(object sender, EventArgs e)
    {
        _autocadGuard.Run(() => this.HandleAutocadDocumentSwitched(sender, e),
            nameof(this.AutocadDocumentSwitched));
    }

    /// <summary>
    /// Syncs units and requests the preview materials for the newly activated document.
    /// </summary>
    private void HandleAutocadDocumentSwitched(object sender, EventArgs e)
    {
        if (ApplicationState.IsShuttingDown) return;

        this.UpdateUnitSystem(sender, e);

        var document = this.AutoCadInstance.ActiveDocument;

        if (document == null) return;

        this.EnsurePreviewMaterials(document);
    }

    /// <inheritdoc />
    public void UpdatePreviewColors(int rhinoColorIndex, int grasshopperColorIndex,
        int selectedColorIndex)
    {
        if (ApplicationState.IsShuttingDown) return;

        this.RhinoPreviewServer.UnSelectedSettings.ColorIndex = rhinoColorIndex;

        this.GrasshopperPreviewServer.UnSelectedSettings.ColorIndex = grasshopperColorIndex;

        // The two servers share one settings instance for the selected state, so it is only
        // set once.
        this.RhinoPreviewServer.SelectedSettings.ColorIndex = selectedColorIndex;

        var document = this.AutoCadInstance.ActiveDocument;

        // Each color has its own material, so setting the color above dropped the old one and
        // the material for the new color has to be created before previews can be shaded with
        // it. That is scheduled rather than done here: previews restyle immediately in the new
        // color and pick the material up when the scheduler has created it. Without an open
        // document there is nothing to draw into either, and the materials are requested again
        // when a document is next activated.
        if (document != null)
        {
            this.EnsurePreviewMaterials(document);
        }

        this.RefreshPreviewAppearance();
    }

    /// <summary>
    /// Handles AutoCAD document changes and responds to them in Grasshopper.
    /// </summary>
    private void AutocadDocumentChange(object sender, IAutocadDocumentChangeEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            _grasshopperChangeResponder.Respond(e.Change);
        }, nameof(this.AutocadDocumentChange));
    }

    /// <summary>
    /// Removes the preview of a Grasshopper object from the <see cref="GrasshopperPreviewServer"/>
    /// when it is removed from the Grasshopper document.
    /// </summary>
    private void OnGrasshopperObjectRemoved(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);
        }, nameof(this.OnGrasshopperObjectRemoved));
    }

    /// <summary>
    /// Updates the preview of a Grasshopper object in the <see cref="GrasshopperPreviewServer"/>
    /// when its preview expires or its selection state changes.
    /// </summary>
    private void UpdateGrasshopperPreview(IGH_DocumentObject ghDocumentObject)
    {
        var instanceGuid = ghDocumentObject.InstanceGuid;

        this.GrasshopperPreviewServer.RemoveObject(ghDocumentObject.InstanceGuid);

        var previewGeometryData = _grasshopperGeometryExtractor.ExtractPreviewGeometry(ghDocumentObject);

        this.GrasshopperPreviewServer.AddObject(instanceGuid, previewGeometryData);
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Grasshopper object's selection state changes.
    /// </summary>
    private void OnGrasshopperSelectionChanged(object? sender, IGrasshopperSelectionEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            foreach (var ghDocumentObject in e.Objects)
            {
                this.UpdateGrasshopperPreview(ghDocumentObject);
            }

            this.EnsurePreviewMaterials();

            this.AutoCadInstance.ActiveDocument?.UpdateEditorScreen();
        }, nameof(this.OnGrasshopperSelectionChanged));
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Grasshopper object's preview expires.
    /// </summary>
    private void OnUpdateGrasshopperPreview(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            var ghDocumentObject = e.GrasshopperObject;

            this.UpdateGrasshopperPreview(ghDocumentObject);

            this.EnsurePreviewMaterials();

            this.AutoCadInstance.ActiveDocument?.UpdateEditorScreen();
        }, nameof(this.OnUpdateGrasshopperPreview));
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is removed.
    /// </summary>
    private void RhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            var rhinoObject = e.RhinoObject;

            this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

            this.AutoCadInstance.ActiveDocument?.UpdateEditorScreen();
        }, nameof(this.RhinoObjectRemoved));
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is modified or appended.
    /// </summary>
    private void RhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            var rhinoObject = e.RhinoObject;

            this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

            if (_rhinoConvertibleFactory.MakeConvertible(rhinoObject.Geometry, out var rhinoConvertible))
            {
                var newSet = new RhinoConvertibleSet { rhinoConvertible };

                this.RhinoPreviewServer.AddObject(rhinoObject.Id, newSet, rhinoObject.IsSelected(false) > 0);
            }

            this.EnsurePreviewMaterials();

            this.AutoCadInstance.ActiveDocument?.UpdateEditorScreen();
        }, nameof(this.RhinoObjectModifiedOrAppended));
    }

    /// <summary>
    /// Deselects all objects in the Rhino preview server when all objects are deselected in Rhino.
    /// </summary>
    private void DeselectAllRhinoPreview(object? sender, EventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            this.RhinoPreviewServer.DeselectAll();

            this.AutoCadInstance.ActiveDocument?.UpdateEditorScreen();
        }, nameof(this.DeselectAllRhinoPreview));
    }

    private void UpdateUnitSystem(object sender, EventArgs e)
    {
        _autocadGuard.Run(() =>
        {
            if (ApplicationState.IsShuttingDown) return;

            var autoCadUnits = new UnitScale(this.AutoCadInstance.ActiveDocument?.UnitSystem ?? _defaultUnitSystem);
            var rhinoUnits = new UnitScale(this.RhinoInstance.ActiveDoc?.ModelUnitSystem ?? _defaultUnitSystem);

            if (autoCadUnits.IsEqualTo(this.UnitConverter.AutoCadUnits) == false ||
                rhinoUnits.IsEqualTo(this.UnitConverter.RhinoUnits) == false)
            {
                UnitConverterClass.Initialize(autoCadUnits, rhinoUnits);
                this.UnitConverter = UnitConverterClass.Instance!;
            }
        }, nameof(this.UpdateUnitSystem));
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        System.Diagnostics.Debug.WriteLine("=== RhinoInsideManager.Shutdown() START ===");

        this.GrasshopperInstance.PreviewExpired -= this.OnUpdateGrasshopperPreview;
        this.GrasshopperInstance.ObjectRemoved -= this.OnGrasshopperObjectRemoved;
        this.GrasshopperInstance.ComponentSelectionChanged -=
            this.OnGrasshopperSelectionChanged;

        this.RhinoInstance.DocumentCreated -= this.UpdateUnitSystem;
        this.RhinoInstance.UnitsChanged -= this.UpdateUnitSystem;
        this.RhinoInstance.ObjectModifiedOrAppended -= this.RhinoObjectModifiedOrAppended;
        this.RhinoInstance.ObjectRemoved -= this.RhinoObjectRemoved;
        this.RhinoInstance.DeselectAll -= this.DeselectAllRhinoPreview;

        this.AutoCadInstance.DocumentActivated -= this.AutocadDocumentSwitched;
        this.AutoCadInstance.UnitsChanged -= this.UpdateUnitSystem;
        this.AutoCadInstance.DocumentChanged -= this.AutocadDocumentChange;

        _previewMaterialScheduler.Shutdown();

        // Clear preview servers with isolated exception handling
        try
        {
            (this.GrasshopperPreviewServer as GrasshopperObjectPreviewServer)?.ClearAll();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GrasshopperPreviewServer cleanup failed: {ex.Message}");
        }

        try
        {
            (this.RhinoPreviewServer as RhinoObjectPreviewServer)?.ClearAll();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RhinoPreviewServer cleanup failed: {ex.Message}");
        }

        // Shutdown instances with isolated exception handling
        try
        {
            this.GrasshopperInstance.Shutdown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GrasshopperInstance shutdown failed: {ex.Message}");
        }

        try
        {
            this.RhinoInstance.Shutdown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RhinoInstance shutdown failed: {ex.Message}");
        }

        try
        {
            this.AutoCadInstance.Shutdown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AutoCadInstance shutdown failed: {ex.Message}");
        }

        System.Diagnostics.Debug.WriteLine("=== RhinoInsideManager.Shutdown() END ===");
    }
}