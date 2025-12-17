using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD points.
/// </summary>
public class Param_AutocadPoint : Param_AutocadObjectBase<GH_AutocadPoint, CadPoint>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("a7a5325e-bfc4-4b79-adaf-863f3fae0123");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadPoint;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Point";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Points";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadPoint"/> class.
    /// </summary>
    public Param_AutocadPoint()
        : base("AutoCAD Point", "AC-Point",
            "A Point in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new PointFilter();

    /// <inheritdoc />
    protected override GH_AutocadPoint WrapEntity(CadPoint entity) => new GH_AutocadPoint(entity);
}