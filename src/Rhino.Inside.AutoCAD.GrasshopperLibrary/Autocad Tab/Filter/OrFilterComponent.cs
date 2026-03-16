using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that combines two filters using a logical OR operation.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class OrFilterComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("a0bf2ca1-5896-4e1e-8b59-a339c69e0870");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.OrFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrFilterComponent"/> class.
    /// </summary>
    public OrFilterComponent()
        : base("Autocad Or Filter", "AC-OrFilter",
            "Combines two filters using a logical OR operation. Objects that satisfy either filter will be selected.",
            "AutoCAD", "Filter")
    { }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter A",
            "A", "The first filter in the logical OR operation.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter B",
            "B", "The second filter in the logical OR operation.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "The combined filter using logical OR.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_AutocadFilter? filterAGoo = null;
        if (!DA.GetData(0, ref filterAGoo) || filterAGoo?.Value == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Filter A is required.");
            return;
        }

        GH_AutocadFilter? filterBGoo = null;
        if (!DA.GetData(1, ref filterBGoo) || filterBGoo?.Value == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Filter B is required.");
            return;
        }

        var filterA = filterAGoo.Value;
        var filterB = filterBGoo.Value;

        var orFilter = new OrFilter(filterA, filterB);

        DA.SetData(0, new GH_AutocadFilter(orFilter));
    }
}
