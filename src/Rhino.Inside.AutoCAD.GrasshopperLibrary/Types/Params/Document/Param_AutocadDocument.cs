using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD curves.
/// </summary>
public class Param_AutocadDocument : GH_Param<GH_AutocadDocument>
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("D1E3C4F2-4B6A-4F8E-9D3A-2B8F5E6C7D8E");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDocument"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadDocument(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDocument"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadDocument(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadCurve"/> class.
    /// </summary>
    public Param_AutocadDocument(GH_ParamAccess access)
        : base("AutoCAD Document", "Document",
            "A Document in AutoCAD", "Params", "AutoCAD", access)
    { }
}