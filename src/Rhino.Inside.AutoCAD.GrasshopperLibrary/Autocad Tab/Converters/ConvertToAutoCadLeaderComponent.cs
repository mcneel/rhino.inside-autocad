using Grasshopper.Kernel;
using Rhino.Geometry;

using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino Leader to an AutoCAD MLeader.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class ConvertToAutoCadLeaderComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("f9b3c6d7-2a8e-4d0f-e4b1-3c7d6e5f8a9b");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadLeaderComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadLeaderComponent"/> class.
    /// </summary>
    public ConvertToAutoCadLeaderComponent()
        : base("To AutoCAD Leader", "AC-ToLdr",
            "Converts a Rhino Leader to an AutoCAD Leader",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGeometryParameter("Leader", "L", "A Rhino Leader", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLeader(), "Leader", "L", "AutoCAD Leader",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        Leader? rhinoLeader = null;

        if (!DA.GetData(0, ref rhinoLeader)
            || rhinoLeader is null) return;

        var cadLeader = _geometryConverter.ToAutoCadType(rhinoLeader);

        if (cadLeader == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert leader to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadLeader(cadLeader);
        DA.SetData(0, goo);
    }
}
