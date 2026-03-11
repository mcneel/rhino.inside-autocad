using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD dictionary entries.
/// </summary>
public class Param_AutocadDictionaryEntry : GH_Param<GH_AutocadDictionaryEntry>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("96332472-fd25-4a82-af14-ad893600aad8");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadDictionaryEntry;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionaryEntry"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadDictionaryEntry(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionaryEntry"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadDictionaryEntry(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionaryEntry"/> class.
    /// </summary>
    public Param_AutocadDictionaryEntry(GH_ParamAccess access)
        : base("Dictionary Entry", "Entry",
            "An entry in an AutoCAD dictionary", "Params", "AutoCAD", access)
    { }
}
