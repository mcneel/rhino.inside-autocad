using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilFeatureLine = Autodesk.Civil.DatabaseServices.FeatureLine;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for standalone Civil 3D Feature Lines.
/// </summary>
public class Param_CivilFeatureLine : Param_AutocadObjectBase<GH_CivilFeatureLine, CivilFeatureLine>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A3D7C2E1-58B4-4F6A-9E03-1B2C4D6E8F90");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Civil.GrasshopperLibrary.Properties.Resources.Param_CivilFeatureLine;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Civil3d Feature Line";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Civil3d Feature Lines";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_CivilFeatureLine"/> class.
    /// </summary>
    public Param_CivilFeatureLine()
        : base("Civil3d Feature Line", "CVL-FL",
            "A standalone Civil 3D Feature Line", "Params", "Civil3d")
    { }

    /// <inheritdoc />
    protected override IObjectFilter CreateSelectionFilter() => new CivilFeatureLineFilter();

    /// <inheritdoc />
    protected override GH_CivilFeatureLine WrapEntity(CivilFeatureLine entity)
    {
        return new GH_CivilFeatureLine(entity);
    }
}
