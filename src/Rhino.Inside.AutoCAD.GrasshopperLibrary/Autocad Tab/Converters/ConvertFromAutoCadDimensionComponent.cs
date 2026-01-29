using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using AutocadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD Dimension to a Rhino Dimension.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadDimensionComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("b5d9e2f3-8c4a-4f6b-a0d7-9e3f2b1c4d5e");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadDimensionComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadDimensionComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadDimensionComponent()
        : base("From AutoCAD Dimension", "AC-FrDim",
            "Converts an AutoCAD Dimension to a Rhino Dimension",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDimension(), "Dimension", "D", "AutoCAD Dimension", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGeometryParameter("Dimension", "D", "A Rhino Dimension", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDimension? autocadDimension = null;

        if (!DA.GetData(0, ref autocadDimension)
            || autocadDimension is null) return;

        var rhinoDimension = _geometryConverter.ToRhinoType(autocadDimension);

        if (rhinoDimension == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert dimension to Rhino format");
            return;
        }

        DA.SetData(0, rhinoDimension);
    }
}
