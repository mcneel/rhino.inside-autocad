using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD line patterns.
/// </summary>
public class Param_AutocadLinePattern : GH_Param<GH_AutocadLinePattern>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("b5e9f8d3-5c2f-4a0b-9d6e-3f8b0c4d7e5a");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadLinePattern;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLinePattern"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadLinePattern(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLinePattern"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadLinePattern(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLinePattern"/> class.
    /// </summary>
    public Param_AutocadLinePattern(GH_ParamAccess access)
        : base("AutoCAD Line Pattern", "LinePattern",
            "A Line Pattern in AutoCAD", "Params", "AutoCAD", access)
    { }
}
