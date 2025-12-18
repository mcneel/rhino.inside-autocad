using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD dynamic block reference properties.
/// </summary>
public class Param_DynamicBlockReferenceProperty : GH_Param<GH_DynamicBlockReferenceProperty>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A7B3C9D1-5E2F-4A8B-9C6D-3E1F7A8B2C4D");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.hidden;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_DynamicBlockReferenceProperty"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_DynamicBlockReferenceProperty(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_DynamicBlockReferenceProperty"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_DynamicBlockReferenceProperty(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_DynamicBlockReferenceProperty"/> class.
    /// </summary>
    public Param_DynamicBlockReferenceProperty(GH_ParamAccess access)
        : base("Dynamic Block Property", "DynProp",
            "A dynamic block reference property in AutoCAD", "Params", "AutoCAD", access)
    { }
}
