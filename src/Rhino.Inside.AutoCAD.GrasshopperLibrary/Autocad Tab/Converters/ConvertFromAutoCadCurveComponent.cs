using Grasshopper.Kernel;
using AutocadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD curve to a Rhino curve
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadCurveComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("522ea559-c8e5-41d0-a559-ef2376898033");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadCurveComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadCurveComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadCurveComponent()
        : base("From AutoCAD Curve", "AC-FrCrv",
            "Converts an AutoCAD to a Rhino curve",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {

        pManager.AddParameter(new Param_AutocadCurve(), "Curve", "C", "AutoCAD curve", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddCurveParameter("Curve", "C", "A Rhino Curve", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadCurve? autocadCurve = null;

        if (!DA.GetData(0, ref autocadCurve)
            || autocadCurve is null) return;

        var rhinoCurve = autocadCurve.ToRhinoCurve();

        if (rhinoCurve == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert curve to Rhino format");
            return;
        }

        DA.SetData(0, rhinoCurve);
    }
}