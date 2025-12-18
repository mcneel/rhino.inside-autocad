using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino brep to an AutoCAD Solid3d.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadSolidComponent : RhinoInsideAutocad_ComponentBase
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

        var goo = new GH_AutocadBrepProxy(rhinoBrep);

        DA.SetData(0, goo);

    }
}
