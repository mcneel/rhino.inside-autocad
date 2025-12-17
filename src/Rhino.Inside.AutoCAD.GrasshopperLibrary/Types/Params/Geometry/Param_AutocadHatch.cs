using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD hatches.
/// </summary>
public class Param_AutocadHatch : Param_AutocadObjectBase<GH_AutocadHatch, CadHatch>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("7521fd8a-9f07-417f-ab8f-3b6f06d8fbe8");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadHatch;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Hatch";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Hatches";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadHatch"/> class.
    /// </summary>
    public Param_AutocadHatch()
        : base("AutoCAD Hatch", "AC-Hatch",
            "A Hatch in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new HatchFilter();

    /// <inheritdoc />
    protected override GH_AutocadHatch WrapEntity(CadHatch entity) => new GH_AutocadHatch(entity);

}
