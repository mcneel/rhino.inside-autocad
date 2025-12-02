using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Mesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private readonly RhinoObjectConverter _rhinoObjectConvert = RhinoObjectConverter.Instance!;

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

        this.UnitSystemManager = unitsSystemManager;

    }

    /// <summary>
    /// Removes the preview of a Grasshopper object from the <see cref="GrasshopperPreviewRegister"/>
    /// when it is removed from the Grasshopper document.
    /// </summary>
    private void OnGrasshopperObjectRemoved(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);
    }

    /// <summary>
    /// Extracts geometry data from a Grasshopper parameter and adds it to the preview data.
    /// </summary>
    /// <param name="param">The Grasshopper parameter to extract geometry from.</param>
    /// <param name="data">The container for the extracted preview data.</param>
    private void ExtractGeometryFromParameter(IGH_Param param, IGrasshopperPreviewData data)
    {
        foreach (var goo in param.VolatileData.AllData(true))
        {
            if (goo is not IGH_PreviewObject) continue;

            if (goo is GH_Curve curve)
            {
                data.Wires.Add(curve.Value);
            }

            if (goo is GH_Brep brep)
            {
                var meshes = Mesh.CreateFromBrep(brep.Value, MeshingParameters.Default);
                data.Meshes.AddRange(meshes);
            }

            if (goo is GH_Mesh mesh)
            {
                data.Meshes.Add(mesh.Value);
            }
        }
    }

    /// <summary>
    /// Extracts preview geometry data from a Grasshopper document object.
    /// </summary>
    /// <param name="ghDocumentObject">The Grasshopper document object to extract geometry from.</param>
    /// <returns>The extracted preview geometry data.</returns>
    private IGrasshopperPreviewData ExtractPreviewGeometry(IGH_DocumentObject ghDocumentObject)
    {
        var previewGeometryData = new GrasshopperPreviewData();

        if (ghDocumentObject is IGH_Component component)
        {
            foreach (var outputParam in component.Params.Output)
            {
                this.ExtractGeometryFromParameter(outputParam, previewGeometryData);
            }

            return previewGeometryData;
        }

        if (ghDocumentObject is IGH_Param param)
        {
            this.ExtractGeometryFromParameter(param, previewGeometryData);
        }

        return previewGeometryData;
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Grasshopper object's preview expires.
    /// </summary>
    private void UpdateGrasshopperPreview(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        var ghDocumentObject = e.GrasshopperObject;

        var instanceGuid = ghDocumentObject.InstanceGuid;

        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);

        var previewGeometryData = this.ExtractPreviewGeometry(ghDocumentObject);

        var convertedEntities = previewGeometryData.GetEntities();

        this.GrasshopperPreviewServer.AddObject(instanceGuid, convertedEntities);

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