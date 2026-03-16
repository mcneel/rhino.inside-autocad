using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that combines two filters using a logical AND operation.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class AndFilterComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("7d4b2aab-3abb-4fc0-8de3-a193cf3a8afb");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AndFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndFilterComponent"/> class.
    /// </summary>
    public AndFilterComponent()
        : base("Autocad And Filter", "AC-AndFilter",
            "Combines two filters using a logical AND operation. Only objects that satisfy both filters will be selected.",
            "AutoCAD", "Filter")
    { }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter A",
            "A", "The first filter in the logical AND operation.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter B",
            "B", "The second filter in the logical AND operation.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "The combined filter using logical AND.", GH_ParamAccess.item);
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

        var andFilter = new AndFilter(filterA, filterB);

        DA.SetData(0, new GH_AutocadFilter(andFilter));
    }
}
