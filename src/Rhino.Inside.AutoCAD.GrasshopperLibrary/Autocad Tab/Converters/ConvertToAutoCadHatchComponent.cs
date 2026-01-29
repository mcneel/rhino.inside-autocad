using Grasshopper.Kernel;

using Rhino.Inside.AutoCAD.Interop;
using RhinoHatch = Rhino.Geometry.Hatch;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino hatch to an AutoCAD hatch.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadHatchComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("38d9b037-b0ce-41cb-abe8-b8dc4bbba9af");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadHatchComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadHatchComponent"/> class.
    /// </summary>
    public ConvertToAutoCadHatchComponent()
        : base("To AutoCAD Hatch", "AC-ToHat",
            "Converts a Rhino Hatch to an AutoCAD Hatch",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGeometryParameter("Hatch", "H", "A Rhino Hatch", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadHatch(), "Hatch", "H", "AutoCAD hatch",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        RhinoHatch? rhinoHatch = null;

        if (!DA.GetData(0, ref rhinoHatch)
            || rhinoHatch is null) return;

        var cadHatch = _geometryConverter.ToAutoCadType(rhinoHatch, null!);

        if (cadHatch == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert hatch to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadHatch(cadHatch);
        DA.SetData(0, goo);
    }
}
