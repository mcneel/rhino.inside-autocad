using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Interop;
using Color = System.Drawing.Color;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
public class CreateAutocadLayerComponent : GH_Component
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("e5b9283d-5312-4c6a-8a1f-a0504257c52b");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateAutocadLayerComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public CreateAutocadLayerComponent()
        : base("CreateAutocadLayer", "CreateLayer",
            "Creates a new AutoCAD Layer",
            "AutoCAD", "Layers")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);

        pManager.AddTextParameter("NewName", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddColourParameter("NewColour", "Colour",
            "The color associated with the layer",
            GH_ParamAccess.item, Color.White);
        pManager[2].Optional = true;

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.item), "LinePatternId", "LinePatternId",
            "The Id of Line Patten of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddColourParameter("Colour", "Colour",
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
        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        if (!DA.GetData(1, ref newName)
            || newName is null) return;

        var newColor = Color.White;
        DA.GetData(2, ref newColor);

        var cadColor = new InternalColor(newColor);

        if (autocadDocument.LayerRepository.TryAddLayer(cadColor, newName, out var autocadLayer) == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to create layer");
            return;
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
