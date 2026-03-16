using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD block attribute reference properties.
/// </summary>
public class Param_BlockAttributeReference : GH_Param<GH_BlockAttributeReference>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("3f32e3e3-b3ce-4033-8ab2-3bc17570c102");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.hidden;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_BlockAttributeReference;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BlockAttributeReference"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_BlockAttributeReference(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BlockAttributeReference"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_BlockAttributeReference(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BlockAttributeReference"/> class.
    /// </summary>
    public Param_BlockAttributeReference(GH_ParamAccess access)
        : base("Block Attributes", "Attr",
            "A block attribute reference in AutoCAD", "Params", "AutoCAD", access)
    { }
}