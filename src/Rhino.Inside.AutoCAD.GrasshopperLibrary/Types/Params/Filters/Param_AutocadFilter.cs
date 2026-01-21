using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD selection filters.
/// </summary>
public class Param_AutocadFilter : GH_Param<GH_AutocadFilter>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("B4C6D8E0-F2A4-4B6C-9D8E-0F2A4B6C8D9E");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadFilter"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadFilter(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadFilter"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadFilter(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadFilter"/> class.
    /// </summary>
    public Param_AutocadFilter(GH_ParamAccess access)
        : base("AutoCAD Filter", "AC-Filter",
            "An AutoCAD selection filter", "Params", "AutoCAD", access)
    { }
}