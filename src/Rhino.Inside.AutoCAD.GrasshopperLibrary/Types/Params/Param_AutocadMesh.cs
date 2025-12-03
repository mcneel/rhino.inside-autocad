using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop.Filters;
using CadMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD polyface meshes.
/// </summary>
public class Param_AutocadMesh : Param_AutocadObjectBase<GH_AutocadMesh, CadMesh>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("c9e3f8d4-5e02-4b6f-a2d0-4f9c7b3e8a5d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadMesh;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Mesh";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Meshes";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadMesh"/> class.
    /// </summary>
    public Param_AutocadMesh()
        : base("AutoCAD Mesh", "Mesh",
            "A Polyface Mesh in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new MeshFilter();

    /// <inheritdoc />
    protected override GH_AutocadMesh WrapEntity(CadMesh entity) => new GH_AutocadMesh(entity);
}
