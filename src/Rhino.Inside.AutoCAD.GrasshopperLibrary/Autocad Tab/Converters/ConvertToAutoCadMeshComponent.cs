using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts a Rhino mesh to an AutoCAD PolyFaceMesh.
/// </summary>
public class ConvertToAutoCadMeshComponent : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("7f3e9a2b-5c1d-4e8f-9b2a-3d6c8e4f1a7b");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertToAutoCadMeshComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToAutoCadMeshComponent"/> class.
    /// </summary>
    public ConvertToAutoCadMeshComponent()
        : base("To AutoCAD Mesh", "ToMsh",
            "Converts a Rhino Mesh to AutoCAD PolyFaceMesh",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddMeshParameter("Mesh", "M", "A Rhino Mesh", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadMesh(), "Mesh", "M", "AutoCAD mesh",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        RhinoMesh? rhinoMesh = null;

        if (!DA.GetData(0, ref rhinoMesh)
        || rhinoMesh is null) return;

        var cadMesh = _geometryConverter.ToAutoCadType(rhinoMesh);

        if (cadMesh == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert mesh to AutoCAD format");
            return;
        }

        var goo = new GH_AutocadMesh(cadMesh);
        DA.SetData(0, goo);
    }
}
