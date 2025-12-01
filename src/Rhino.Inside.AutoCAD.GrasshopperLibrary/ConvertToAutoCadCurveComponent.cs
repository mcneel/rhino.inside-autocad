using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

public class ConvertToAutoCadCurveComponent : GH_Component
{
    private GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    public ConvertToAutoCadCurveComponent()
        : base("ToAutoCadCurve", "ToCadCurve",
            "Converts a Rhino Curve to AutoCAD curve",
            "AutoCAD", "Convert")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Curve", "C", "A Rhino Curve", GH_ParamAccess.item);

    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Curve", "C", "A AutoCAD Curve", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        RhinoCurve? rhinoCurve = null;

        if (!DA.GetData(0, ref rhinoCurve)) return;
        if (rhinoCurve is null) return;

        var cadCurve = _geometryConverter.ToAutoCadType(rhinoCurve);

        DA.SetDataList(0, cadCurve);
    }

    public override Guid ComponentGuid
    {
        get { return new Guid("12345678-1234-1234-1234-123456789ABC"); }
    }

    protected override System.Drawing.Bitmap Icon
    {
        get { return null; }
    }
}