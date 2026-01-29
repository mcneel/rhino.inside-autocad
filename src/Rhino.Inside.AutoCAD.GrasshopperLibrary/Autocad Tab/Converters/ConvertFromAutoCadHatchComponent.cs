using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD hatch to a Rhino hatch
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadHatchComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("83e7d2d6-915e-45e4-887d-4cbe19b38d93");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadHatchComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadHatchComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadHatchComponent()
        : base("From AutoCAD Hatch", "AC-FrHat",
            "Converts an AutoCAD Hatch to a Rhino Hatch",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadHatch(), "Hatch", "H", "AutoCAD hatch", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGeometryParameter("Hatch", "H", "A Rhino Hatch", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        CadHatch? autocadHatch = null;

        if (!DA.GetData(0, ref autocadHatch)
            || autocadHatch is null) return;

        var rhinoHatch = _geometryConverter.ToRhinoType(autocadHatch);

        if (rhinoHatch == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert hatch to Rhino format");
            return;
        }

        DA.SetData(0, rhinoHatch);
    }
}
