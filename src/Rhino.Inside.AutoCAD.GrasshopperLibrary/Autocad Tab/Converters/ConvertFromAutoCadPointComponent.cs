using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using AutocadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD DBPoint to a Rhino point.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadPointComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("6d8e1f2a-5b7c-4d9e-2f3a-7b9c5d1e8f4a");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadPointComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadPointComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadPointComponent()
        : base("From AutoCAD Point", "AC-FrPt",
            "Converts an AutoCAD DBPoint to a Rhino point",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadPoint(), "Point", "P", "AutoCAD point", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddPointParameter("Point", "P", "A Rhino Point", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadPoint? autocadPoint = null;

        if (!DA.GetData(0, ref autocadPoint)
            || autocadPoint is null) return;

        var rhinoPoint = _geometryConverter.ToRhinoType(autocadPoint);

        DA.SetData(0, rhinoPoint.Location);
    }
}
