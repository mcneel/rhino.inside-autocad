using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD bake settings.
/// </summary>
public class Param_BakeSettings : GH_Param<GH_BakeSettings>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A3B5C7D9-E1F3-4A5B-8C7D-9E1F3A5B7C9D");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_BakeSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BakeSettings"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_BakeSettings(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BakeSettings"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_BakeSettings(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_BakeSettings"/> class.
    /// </summary>
    public Param_BakeSettings(GH_ParamAccess access)
        : base("Bake Settings", "Settings",
            "Settings for baking objects to AutoCAD", "Params", "AutoCAD", access)
    { }
}
