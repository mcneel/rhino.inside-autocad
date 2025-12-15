using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino brep to an AutoCAD Solid3d.
/// </summary>
public class ConvertToAutoCadSolidComponent : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("5b7c9d1e-4f6a-3b8c-7d2e-9f1a5b6c8d3e");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadSolidComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadSolidComponent"/> class.
    /// </summary>
    public ConvertToAutoCadSolidComponent()
        : base("To AutoCAD Solid", "AC-ToSld",
            "Converts a Rhino Brep to AutoCAD Solid3d",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddBrepParameter("Brep", "B", "A Rhino Brep", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadSolid(), "Solid", "S", "AutoCAD solid",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        RhinoBrep? rhinoBrep = null;

        if (!DA.GetData(0, ref rhinoBrep)
        || rhinoBrep is null) return;

        var cadSolids = _geometryConverter.ToAutoCadType(rhinoBrep);

        if (cadSolids == null || cadSolids.Length == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert brep to AutoCAD format");
            return;
        }

        if (cadSolids.Length > 1)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                $"Brep converted to {cadSolids.Length} AutoCAD solids. Only returning the first one.");
        }

        var goo = new GH_AutocadSolid(cadSolids[0]);
        DA.SetData(0, goo);
    }
}
