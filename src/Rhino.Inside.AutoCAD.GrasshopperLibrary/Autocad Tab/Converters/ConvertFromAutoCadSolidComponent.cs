using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD Solid3d to a Rhino brep.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadSolidComponent : RhinoInsideAutocad_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("9e2f4a6b-8c1d-5e7f-3a9b-6c8d2e5f9a1b");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadSolidComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadSolidComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadSolidComponent()
        : base("From AutoCAD Solid", "AC-FrSld",
            "Converts an AutoCAD Solid3d to a Rhino brep",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadSolid(), "Solid", "S", "AutoCAD solid", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddBrepParameter("Brep", "B", "A Rhino Brep", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        Brep? rhinoBrep = null;

        if (!DA.GetData(0, ref rhinoBrep)
            || rhinoBrep is null) return;

        DA.SetData(0, rhinoBrep);
    }
}
