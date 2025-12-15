using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns information about an AutoCAD Line Pattern.
/// </summary>
public class AutocadLineTypeComponent : GH_Component
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d5e7f0a1-6b4c-4d8e-0f3a-2b9c4d6e8f0b");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadLineTypeComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadLineTypeComponent"/> class.
    /// </summary>
    public AutocadLineTypeComponent()
        : base("AutoCAD Line Type", "AC-LType",
            "Gets Information from an AutoCAD Line Type Pattern",
            "AutoCAD", "LineTypes")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.item), "LinePattern",
            "Pattern", "An AutoCAD Line Pattern", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Line Pattern.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Line Pattern.", GH_ParamAccess.item);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadLinetypeTableRecord? linePattern = null;

        if (!DA.GetData(0, ref linePattern)
            || linePattern is null) return;

        var id = linePattern.Id;
        var name = linePattern.Name;

        DA.SetData(0, name);
        DA.SetData(1, id);

    }
}
