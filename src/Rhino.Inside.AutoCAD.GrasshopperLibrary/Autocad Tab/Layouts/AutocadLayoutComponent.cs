using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that sets properties for an AutoCAD layout.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.5")]
public class AutocadLayoutComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("1a15933d-7233-47e3-83d3-192d10cb80bb");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadLayoutInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadLayoutComponent"/> class.
    /// </summary>
    public AutocadLayoutComponent()
        : base("AutoCAD Layout", "AC-Lay",
            "Gets Information from an AutoCAD Layout",
            "AutoCAD", "Layouts")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayout(GH_ParamAccess.item), "Layout",
            "Layout", "An AutoCAD Layout", GH_ParamAccess.item);

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layout", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layout", GH_ParamAccess.item);

        pManager.AddIntegerParameter("TabOrder", "TabOrder",
            "The tab display order", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "BlockTableRecordId", "BlockId",
            "The associated block table record ID", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadLayoutWrapper? layout = null;

        if (!DA.GetData(0, ref layout) || layout is null) return;

        // Output updated values
        DA.SetData(0, layout.Name);
        DA.SetData(1, layout.Id);
        DA.SetData(2, layout.TabOrder);
        DA.SetData(3, layout.BlockTableRecordId);
    }
}
