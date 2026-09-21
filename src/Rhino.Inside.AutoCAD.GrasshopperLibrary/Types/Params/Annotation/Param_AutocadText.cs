using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadMText = Autodesk.AutoCAD.DatabaseServices.MText;
using CadText = Autodesk.AutoCAD.DatabaseServices.DBText;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD Text.
/// </summary>
public class Param_AutocadText : Param_AutocadObjectBase<GH_AutocadText, CadMText>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("8d029bfd-f9fa-4c15-b849-9212a2b338e1");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadText;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Text Object";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Text Objects";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadSolid"/> class.
    /// </summary>
    public Param_AutocadText()
        : base("AutoCAD Text", "Text",
            "A Text Object in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IObjectFilter CreateSelectionFilter() => new TextFilter();

    /// <inheritdoc />
    protected override GH_AutocadText WrapEntity(CadMText entity) => new GH_AutocadText(entity);

    /// <inheritdoc />
    /// <remarks>
    /// <see cref="TextFilter"/> lets the user pick single line TEXT entities as well as
    /// MTEXT, so a picked <see cref="CadText"/> is converted here rather than being
    /// dropped. The entity is the wrapper around the picked object, so it must be
    /// unwrapped before it is tested against the AutoCAD type.
    /// </remarks>
    protected override bool ConvertSupportObject(IEntity entity, out GH_AutocadText supportedGoo)
    {
        supportedGoo = null!;

        if (entity.Unwrap() is not CadText text) return false;

        supportedGoo = GH_AutocadText.CreateFromTextEntity(text);

        return true;
    }
}
