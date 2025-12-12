using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD Dimensions.
/// </summary>
public class Param_AutocadDimension : Param_AutocadObjectBase<GH_AutocadDimension, CadDimension>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("a4c8e1f2-7b3d-4e5a-9c6f-8d2e1a0b3c4d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadDimension;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Dimension";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Dimensions";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDimension"/> class.
    /// </summary>
    public Param_AutocadDimension()
        : base("AutoCAD Dimension", "Dim",
            "A Dimension in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new DimensionFilter();

    /// <inheritdoc />
    protected override GH_AutocadDimension WrapEntity(CadDimension entity) => new GH_AutocadDimension(entity);
}
