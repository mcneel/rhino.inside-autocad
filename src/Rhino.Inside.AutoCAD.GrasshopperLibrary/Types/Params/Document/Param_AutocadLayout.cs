using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD layouts.
/// </summary>
public class Param_AutocadLayout : GH_Param<GH_AutocadLayout>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("c5f7e9d2-6a8b-4c3d-9f1e-7b4a3d8e6c2f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadLayout;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLayout"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadLayout(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLayout"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadLayout(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLayout"/> class.
    /// </summary>
    public Param_AutocadLayout(GH_ParamAccess access)
        : base("AutoCAD Layout", "Layout",
            "A Layout in AutoCAD", "Params", "AutoCAD", access)
    { }
}
