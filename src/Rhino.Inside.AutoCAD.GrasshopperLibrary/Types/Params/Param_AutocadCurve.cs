using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop.Filters;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD curves.
/// </summary>
public class Param_AutocadCurve : Param_AutocadObjectBase<GH_AutocadCurve, CadCurve>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("54da72a4-8921-4718-83ff-416bcfbc1d8c");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadCurve;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Curve";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Curves";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadCurve"/> class.
    /// </summary>
    public Param_AutocadCurve()
        : base("AutoCAD Curve", "Curve",
            "A Curve in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new CurveFilter();

    /// <inheritdoc />
    protected override GH_AutocadCurve WrapEntity(CadCurve entity) => new GH_AutocadCurve(entity);
}