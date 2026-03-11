using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a TypedValue from a DxfCode and value.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class CreateTypedValueComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("8e5f3b2a-4c1d-9e7f-6a8b-2c3d5e4f1a9b");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateTypedValueComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTypedValueComponent"/> class.
    /// </summary>
    public CreateTypedValueComponent()
        : base("Create Filter Rules", "AC-FilterRules",
            "Creates a FilterRules (Autocad TypedValues) from a DxfCode and value. These can be used to create Custom Filters",
            "AutoCAD", "Filter")
    { }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddIntegerParameter("DxfCode", "C", "The integer which represents the DxfCode, use the DxfCodeValueList for help", GH_ParamAccess.item);
        pManager.AddGenericParameter("Value", "V", "The value for the FilterRules", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadTypedValue(GH_ParamAccess.item),
            "FilterRules", "Rule", "The created FilterRules (Autocad TypedValues)", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var code = 0;
        object? value = null;

        if (!DA.GetData(0, ref code)) return;
        if (!DA.GetData(1, ref value)) return;

        // Unwrap GH types to get raw values
        value = UnwrapGoo(value);

        if (value == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Value cannot be null.");
            return;
        }

        var typedValue = new TypedValue((short)code, value);
        var wrapper = new TypedValueWrapper(typedValue);

        DA.SetData(0, new GH_AutocadTypedValue(wrapper));
    }

    /// <summary>
    /// Unwraps Grasshopper Goo types to get the underlying raw values.
    /// </summary>
    private static object? UnwrapGoo(object? value)
    {
        return value switch
        {
            GH_String ghString => ghString.Value,
            GH_Integer ghInt => ghInt.Value,
            GH_Number ghNumber => ghNumber.Value,
            GH_Boolean ghBool => ghBool.Value,
            IGH_Goo goo => goo.ScriptVariable(),
            _ => value
        };
    }
}
