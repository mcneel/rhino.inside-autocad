using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino Dimension to an AutoCAD Dimension.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadDimensionComponent : RhinoInsideAutocad_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("c6e0f3a4-9d5b-4a7c-b1e8-0f4a3c2d5e6f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadDimensionComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadDimensionComponent"/> class.
    /// </summary>
    public ConvertToAutoCadDimensionComponent()
        : base("To AutoCAD Dimension", "AC-ToDim",
            "Converts a Rhino Dimension to an AutoCAD Dimension",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGeometryParameter("Dimension", "D", "A Rhino Dimension", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDimension(), "Dimension", "D", "AutoCAD Dimension",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        Dimension? rhinoDimension = null;

        if (!DA.GetData(0, ref rhinoDimension)
            || rhinoDimension is null) return;

        var cadDimension = _geometryConverter.ToAutoCadType(rhinoDimension);

        if (cadDimension == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert dimension to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadDimension(cadDimension);
        DA.SetData(0, goo);
    }
}
