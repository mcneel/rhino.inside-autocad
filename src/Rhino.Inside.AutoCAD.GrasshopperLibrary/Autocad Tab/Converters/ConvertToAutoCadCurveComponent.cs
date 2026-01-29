using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino curve to an AutoCAD curve.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadCurveComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("12345678-1234-1234-1234-123456789ABC");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadCurveComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadCurveComponent"/> class.
    /// </summary>
    public ConvertToAutoCadCurveComponent()
        : base("To AutoCAD Curve", "AC-ToCrv",
            "Converts a Rhino Curve to AutoCAD curve",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Curve", "C", "A Rhino Curve", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadCurve(), "Curve", "C", "AutoCAD curve",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        RhinoCurve? rhinoCurve = null;

        if (!DA.GetData(0, ref rhinoCurve)
        || rhinoCurve is null) return;

        var cadCurves = _geometryConverter.ToAutoCadType(rhinoCurve);

        if (cadCurves == null || cadCurves.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert curve to AutoCAD format");
            return;
        }

        var goo = cadCurves.Select(cadCurve => new GH_AutocadCurve(cadCurve));
        DA.SetDataList(0, goo);
    }
}