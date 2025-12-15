using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using Point3d = Rhino.Geometry.Point3d;
using RhinoPoint = Rhino.Geometry.Point;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino point to an AutoCAD DBPoint.
/// </summary>
public class ConvertToAutoCadPointComponent : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("2a4b6c8d-3e5f-4a7b-9c1d-8e2f4a6b7c9d");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadPointComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadPointComponent"/> class.
    /// </summary>
    public ConvertToAutoCadPointComponent()
        : base("To AutoCAD Point", "AC-ToPt",
            "Converts a Rhino Point to AutoCAD DBPoint",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddPointParameter("Point", "P", "A Rhino Point", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadPoint(), "Point", "P", "AutoCAD point",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var rhinoPoint = Point3d.Unset;

        if (!DA.GetData(0, ref rhinoPoint)
        || !rhinoPoint.IsValid) return;

        var rhinoPointGeometry = new RhinoPoint(rhinoPoint);
        var cadPoint = _geometryConverter.ToAutoCadType(rhinoPointGeometry);

        if (cadPoint == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert point to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadPoint(cadPoint);
        DA.SetData(0, goo);
    }
}
