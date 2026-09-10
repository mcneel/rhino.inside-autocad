using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.2")]
public class SetAutocadLayerComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("fb1aba74-b083-43b0-acc2-749eb011617d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadLayerComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public SetAutocadLayerComponent()
        : base("Set AutoCAD Layer", "AC-SetLyr",
            "Sets Information for an AutoCAD Layer",
            "AutoCAD", "Layers")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer",
            "Layer", "An AutoCAD Layer", GH_ParamAccess.item);

        pManager.AddTextParameter("NewName", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "NewLinePatternId", "LinePatternId",
            "The Id of Line Patten of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadColor(GH_ParamAccess.item), "NewColour", "Colour",
            "The color associated with the layer",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Locked", "Locked",
            "Boolean value indicating if the AutoCAD Layers is Locked",
            GH_ParamAccess.item);

        // Make all parameters optional except the first
        pManager[1].Optional = true;
        pManager[2].Optional = true;
        pManager[3].Optional = true;
        pManager[4].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer", "Layer",
            "The updated AutoCAD Layer", GH_ParamAccess.item);

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
        AutocadLayerTableRecordWrapper? autocadLayer = null;

        if (!DA.GetData(0, ref autocadLayer)
            || autocadLayer is null) return;

        var newName = autocadLayer.Name;
        var newPattenId = autocadLayer.LineTypeId;
        var newColor = autocadLayer.Color;
        var newIsLocked = autocadLayer.IsLocked;

        DA.GetData(1, ref newName);
        DA.GetData(2, ref newPattenId);
        DA.GetData(3, ref newColor);
        DA.GetData(4, ref newIsLocked);

        var change = newName != autocadLayer.Name
                     || newPattenId != autocadLayer.LineTypeId
                     || !newColor.IsEqualTo(autocadLayer.Color)
                     || newIsLocked != autocadLayer.IsLocked;

        if (change)
        {
            var document = this.GetDocumentForObjectId(autocadLayer.Id);
            if (document is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
                return;
            }

            var transactionManager = document.CreateTransactionManager();

            autocadLayer = transactionManager.PerformTask(() =>
            {
                var transaction = transactionManager.Unwrap();

                var cadLayer =
                    transaction.GetObject(autocadLayer.Id.Unwrap(), OpenMode.ForWrite) as LayerTableRecord;

                cadLayer!.Name = newName;
                cadLayer.LinetypeObjectId = newPattenId.Unwrap();
                cadLayer.Color = newColor.Unwrap();
                cadLayer.IsLocked = newIsLocked;

                return new AutocadLayerTableRecordWrapper(cadLayer);
            });
        }

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
