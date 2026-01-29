using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using Color = System.Drawing.Color;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.4")]
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

        pManager.AddColourParameter("NewColour", "Colour",
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
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "LinePatternId", "LinePatternId",
            "The Id of Line Patten of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddColourParameter("Colour", "Colour",
            "The color associated with the layer",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Locked", "Locked",
            "Boolean value indicating if the AutoCAD Layers is Locked",
            GH_ParamAccess.item);

    }

    /// <summary>
    /// Updates the properties of an AutoCAD Layer, Return a new Wrapper with updated values.
    /// If the update fails, the original layer is returned and an error message is added
    /// to the component.
    /// </summary>
    private AutocadLayerTableRecordWrapper UpdateLayout(AutocadLayerTableRecordWrapper autocadLayer, string newName,
       IObjectId newPattenId, IColor newColor, bool newIsLocked)
    {
        try
        {
            var cadLayerId = autocadLayer.Id.Unwrap();

            var activeDocument = Application.DocumentManager.MdiActiveDocument;

            using var documentLock = activeDocument.LockDocument();

            var database = activeDocument.Database;

            using var transactionManagerWrapper = new TransactionManagerWrapper(database);

            using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

            var cadLayer =
                transaction.GetObject(cadLayerId, OpenMode.ForWrite) as LayerTableRecord;

            cadLayer!.Name = newName;
            cadLayer.LinetypeObjectId = newPattenId.Unwrap();
            cadLayer.Color =
                Autodesk.AutoCAD.Colors.Color.FromRgb(newColor.Red, newColor.Green,
                    newColor.Blue);
            cadLayer.IsLocked = newIsLocked;

            transaction.Commit();

            activeDocument.Editor.Regen();

            return new AutocadLayerTableRecordWrapper(cadLayer);

        }
        catch (Exception e)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            return autocadLayer;
        }
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadLayerTableRecordWrapper? autocadLayer = null;

        if (!DA.GetData(0, ref autocadLayer)
            || autocadLayer is null) return;

        var newName = autocadLayer.Name;
        var newPattenId = autocadLayer.LinePattenId;
        var newColor = autocadLayer.Color;
        var newIsLocked = autocadLayer.IsLocked;

        DA.GetData(1, ref newName);
        DA.GetData(2, ref newPattenId);
        DA.GetData(3, ref newColor);
        DA.GetData(4, ref newIsLocked);

        var change = newName != autocadLayer.Name
                     || newPattenId != autocadLayer.LinePattenId
                     || newColor != autocadLayer.Color
                     || newIsLocked != autocadLayer.IsLocked;

        if (change)
        {
            autocadLayer = this.UpdateLayout(autocadLayer, newName, newPattenId, newColor, newIsLocked);
        }

        var linePatten = autocadLayer.LinePattenId;

        var name = autocadLayer.Name;

        var id = autocadLayer.Id;

        var color = autocadLayer.Color;

        var gooColor = new GH_Colour(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));

        var isLocked = autocadLayer.IsLocked;

        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetData(2, linePatten);
        DA.SetData(3, gooColor);
        DA.SetData(4, isLocked);
    }
}
