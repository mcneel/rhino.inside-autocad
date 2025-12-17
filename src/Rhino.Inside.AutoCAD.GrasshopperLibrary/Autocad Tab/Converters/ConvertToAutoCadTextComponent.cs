using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino TextEntity to an AutoCAD text.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadTextComponent : RhinoInsideAutocad_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("cd3fae72-b5de-4cc2-b6b3-b11281345624");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadTextComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadTextComponent"/> class.
    /// </summary>
    public ConvertToAutoCadTextComponent()
        : base("To AutoCAD Text", "AC-ToTxt",
            "Converts a Rhino TextEntity to an AutoCAD Text",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGeometryParameter("Text", "T", "A Rhino TextEntity", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadText(), "Text", "T", "AutoCAD text",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        TextEntity? rhinoText = null;

        if (!DA.GetData(0, ref rhinoText)
            || rhinoText is null) return;

        var cadText = _geometryConverter.ToAutoCadType(rhinoText);

        if (cadText == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert text to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadText(cadText);
        DA.SetData(0, goo);
    }
}
