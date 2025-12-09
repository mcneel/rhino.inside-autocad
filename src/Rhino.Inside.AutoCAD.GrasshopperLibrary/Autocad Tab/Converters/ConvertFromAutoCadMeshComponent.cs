using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using AutocadMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD PolyFaceMesh to a Rhino mesh.
/// </summary>
public class ConvertFromAutoCadMeshComponent : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("3c5d7e9f-2a4b-5c6d-8e1f-4a7b9c2d5e8f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadMeshComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadMeshComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadMeshComponent()
        : base("FromAutoCadMesh", "FromCadMesh",
            "Converts an AutoCAD PolyFaceMesh to a Rhino mesh",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadMesh(), "Mesh", "M", "AutoCAD mesh", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddMeshParameter("Mesh", "M", "A Rhino Mesh", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadMesh? autocadMesh = null;

        if (!DA.GetData(0, ref autocadMesh)
            || autocadMesh is null) return;

        var rhinoMesh = _geometryConverter.ToRhinoType(autocadMesh);

        DA.SetData(0, rhinoMesh);
    }
}
