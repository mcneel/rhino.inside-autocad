using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilGrading = Autodesk.Civil.DatabaseServices.Grading;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for Civil 3D Gradings.
/// </summary>
public class Param_CivilGrading : Param_AutocadObjectBase<GH_CivilGrading, CivilGrading>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("C81F4A6D-2E9B-4073-B5A1-9D0E3F7C6B28");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Civil.GrasshopperLibrary.Properties.Resources.Param_CivilGrading;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Civil3d Grading";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Civil3d Gradings";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_CivilGrading"/> class.
    /// </summary>
    public Param_CivilGrading()
        : base("Civil3d Grading", "CVL-Grd",
            "A Civil 3D Grading", "Params", "Civil3d")
    { }

    /// <inheritdoc />
    protected override IObjectFilter CreateSelectionFilter() => new CivilGradingFilter();

    /// <inheritdoc />
    protected override GH_CivilGrading WrapEntity(CivilGrading entity)
    {
        return new GH_CivilGrading(entity);
    }
}
