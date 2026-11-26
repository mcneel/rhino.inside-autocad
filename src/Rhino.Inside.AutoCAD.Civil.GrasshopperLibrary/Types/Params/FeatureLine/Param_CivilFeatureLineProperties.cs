using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for Civil 3D Feature Line properties.
/// </summary>
/// <remarks>
/// This parameter type wraps <see cref="GH_CivilFeatureLineProperties"/> objects which
/// contain properties from Feature Lines.
/// </remarks>
public class Param_CivilFeatureLineProperties : GH_Param<GH_CivilFeatureLineProperties>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("6F1E2A9C-3B47-4D8E-9A21-7C5D0B4E8F13");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_CivilFeatureLineProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_CivilFeatureLineProperties"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_CivilFeatureLineProperties(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_CivilFeatureLineProperties"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_CivilFeatureLineProperties(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_CivilFeatureLineProperties"/> class.
    /// </summary>
    public Param_CivilFeatureLineProperties(GH_ParamAccess access)
        : base("Civil3d Feature Line Properties", "FLProps",
            "Properties from a Civil 3D Feature Line", "Params", "Civil3d", access)
    { }
}
