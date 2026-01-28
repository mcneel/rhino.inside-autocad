using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using AutocadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD MLeader to a Rhino Leader.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertFromAutoCadLeaderComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("e8a2b5c6-1f7d-4c9e-d3a0-2b6c5d4e7f8a");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadLeaderComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadLeaderComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadLeaderComponent()
        : base("From AutoCAD Leader", "AC-FrLdr",
            "Converts an AutoCAD Leader to a Rhino Leader",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLeader(), "Leader", "L", "AutoCAD Leader", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGeometryParameter("Leader", "L", "A Rhino Leader", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadMLeader? autocadLeader = null;

        if (!DA.GetData(0, ref autocadLeader)
            || autocadLeader is null) return;

        var rhinoLeader = _geometryConverter.ToRhinoType(autocadLeader);

        if (rhinoLeader == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert leader to Rhino format");
            return;
        }

        DA.SetData(0, rhinoLeader);
    }
}
