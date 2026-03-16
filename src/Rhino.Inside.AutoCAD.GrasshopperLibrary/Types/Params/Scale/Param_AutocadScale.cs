using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD Scale 3d.
/// </summary>
public class Param_AutocadScale : GH_Param<GH_Scale3d>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("c78a707d-30b1-4869-9c11-9b80d2b10ce3");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.hidden;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadScale;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadScale"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadScale(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadScale"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadScale(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadScale"/> class.
    /// </summary>
    public Param_AutocadScale(GH_ParamAccess access)
        : base("Autocad Scale3d", "Scale3d",
            "A scale 3d AutoCAD", "Params", "AutoCAD", access)
    { }
}