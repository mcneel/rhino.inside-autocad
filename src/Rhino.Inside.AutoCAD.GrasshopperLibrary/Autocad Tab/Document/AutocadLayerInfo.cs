using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Interop;
using Color = System.Drawing.Color;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
public class AutocadLayerInfo : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("4455be27-68f3-4695-b56e-894ab15ae964");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadLayerInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public AutocadLayerInfo()
        : base("AutoCadLayerInfo", "DocInfo",
            "Gets Information from an autocad Layer",
            "AutoCAD", "Layer")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer",
            "Doc", "An AutoCAD Layer", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadLinePattern(GH_ParamAccess.item), "LinePatten", "LinePatten",
            "The LinePatten of the AutoCAD Layer.", GH_ParamAccess.item);

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
        AutocadLayerWrapper? autocadLayer = null;

        if (!DA.GetData(0, ref autocadLayer)
            || autocadLayer is null) return;

        var linePatten = autocadLayer.AutocadLinePattern;

        var name = autocadLayer.Name;

        var id = autocadLayer.Id;

        var color = autocadLayer.Color;

        var gooColor = new GH_Colour(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));

        var isLocked = autocadLayer.IsLocked;

        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetData(1, linePatten);
        DA.SetData(2, gooColor);
        DA.SetData(3, isLocked);
    }
}
