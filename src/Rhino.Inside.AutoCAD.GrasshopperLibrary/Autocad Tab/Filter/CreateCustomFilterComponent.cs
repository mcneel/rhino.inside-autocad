using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a CustomFilter from a list of TypedValues.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class CreateCustomFilterComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateCustomFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCustomFilterComponent"/> class.
    /// </summary>
    public CreateCustomFilterComponent()
        : base("Create Custom Filter", "AC-CustomFilter",
            "Creates a custom selection filter from FilterRules (Autocad TypedValues)",
            "AutoCAD", "Filter")
    { }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadTypedValue(GH_ParamAccess.list),
            "FilterRules", "TV", "The filter criteria as FilterRules (Autocad TypedValues)", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "The created custom filter", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var typedValueGoos = new List<GH_AutocadTypedValue>();
        if (!DA.GetDataList(0, typedValueGoos)) return;

        var typedValues = typedValueGoos
            .Where(g => g?.Value != null)
            .Select(g => g.Value)
            .ToList();

        if (typedValues.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid FilterRules (Autocad TypedValues) provided.");
            return;
        }

        var customFilter = new CustomFilter(typedValues);

        DA.SetData(0, new GH_AutocadFilter(customFilter));
    }
}
