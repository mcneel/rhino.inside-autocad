using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD TypedValue wrappers.
/// </summary>
public class Param_AutocadTypedValue : GH_Param<GH_AutocadTypedValue>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A3B5C7D9-E1F3-4A5B-8C7D-9E1F3A5B7C8D");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadTypedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadTypedValue"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadTypedValue(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadTypedValue"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadTypedValue(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadTypedValue"/> class.
    /// </summary>
    public Param_AutocadTypedValue(GH_ParamAccess access)
        : base("AutoCAD Filter Rule", "AC-FilterRule",
            "AFilterRules (Autocad TypedValues)", "Params", "AutoCAD", access)
    { }
}