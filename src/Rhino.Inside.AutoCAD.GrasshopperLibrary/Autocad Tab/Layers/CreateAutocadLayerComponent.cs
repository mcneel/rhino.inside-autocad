using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.2")]
public class CreateAutocadLayerComponent : Layer_BaseComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("e5b9283d-5312-4c6a-8a1f-a0504257c52b");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateAutocadLayerComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public CreateAutocadLayerComponent()
        : base("Create AutoCAD Layer", "AC-AddLyr",
            "Creates a new AutoCAD Layer",
            "AutoCAD", "Layers")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddTextParameter("NewName", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadColor(GH_ParamAccess.item), "NewColour", "Colour",
            "The color associated with the layer",
            GH_ParamAccess.item);
        pManager[2].Optional = true;

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer", "Layer",
            "The created AutoCAD Layer", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "LinePatternId", "LinePatternId",
            "The Id of Line Patten of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadColor(GH_ParamAccess.item), "Colour", "Colour",
            "The color associated with the layer",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Locked", "Locked",
            "Boolean value indicating if the AutoCAD Layers is Locked",
            GH_ParamAccess.item);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        var newName = string.Empty;
        DA.GetData(0, ref autocadDocument);

        if (autocadDocument is null)
        {
            var activeDoc = RhinoInsideAutoCadExtension.Application?.RhinoInsideManager?.AutoCadInstance?.ActiveDocument;
            if (activeDoc is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
                return;
            }
            autocadDocument = activeDoc as AutocadDocument;
        }

        if (autocadDocument is null)
            return;

        if (!DA.GetData(1, ref newName)
            || newName is null) return;

        AutocadColorWrapper? newColor = null;
        DA.GetData(2, ref newColor);

        var cadColor = newColor ?? AutocadColorWrapper.CreateByLayer();

        var transactionManager = autocadDocument.CreateTransactionManager();

        var autocadLayer = transactionManager.PerformTask(() =>
        {
            if (this.TryGetByName(transactionManager, newName, out var existing))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A Layer with this name already exists");

                return existing;
            }

            return AutocadLayerTableRecordWrapper.Create(autocadDocument, cadColor, newName);
        });

        var linePatten = autocadLayer.LineTypeId;

        var name = autocadLayer.Name;

        var id = autocadLayer.Id;

        var color = autocadLayer.Color;

        var gooColor = new GH_AutocadColor(color);

        var isLocked = autocadLayer.IsLocked;

        var layerGoo = new GH_AutocadLayer(autocadLayer);

        DA.SetData(0, layerGoo);
        DA.SetData(1, name);
        DA.SetData(2, id);
        DA.SetData(3, linePatten);
        DA.SetData(4, gooColor);
        DA.SetData(5, isLocked);
    }
}
