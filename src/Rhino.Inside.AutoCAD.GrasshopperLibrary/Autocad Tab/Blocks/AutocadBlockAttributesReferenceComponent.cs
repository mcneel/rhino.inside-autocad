using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts properties from an AutoCAD block attributes reference.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadBlockAttributesReferenceComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("15262905-a7ca-44ea-8107-290eaee442d0");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBlockAttributesReferenceComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDynamicPropertiesComponent"/> class.
    /// </summary>
    public AutocadBlockAttributesReferenceComponent()
        : base("Block Attributes", "AC-Attr",
            "Gets information from an AutoCAD Block Attribute Reference",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_BlockAttributeReference(GH_ParamAccess.item),
            "Attribute", "Attr", "A Dynamic Block Attribute Reference", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Tag", "T", "The tag of the attribute", GH_ParamAccess.item);
        pManager.AddTextParameter("Value", "V", "The value of the attribute", GH_ParamAccess.item);
        pManager.AddBooleanParameter("Is Multiline", "MText", "Whether the property is a Multiline (MText)", GH_ParamAccess.item);
        pManager.AddPointParameter("Location", "Loc", "The location of attribute reference, converted to Rhino's Units.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AttributeWrapper? attribute = null;

        if (!DA.GetData(0, ref attribute) || attribute is null)
            return;

        DA.SetData(0, attribute.Tag);
        DA.SetData(1, attribute.Text);
        DA.SetData(2, attribute.IsMultiline);
        DA.SetData(3, attribute.AlignmentPoint);
    }
}