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

    /// <summary>
    /// Converts a DBText to an MText.
    /// </summary>
    private CadMText ConvertToMText(CadText dbText)
    {
        var mText = new CadMText();

        mText.Contents = dbText.TextString;

        mText.Location = dbText.Position;

        mText.TextHeight = dbText.Height;

        mText.Rotation = dbText.Rotation;

        mText.TextStyleId = dbText.TextStyleId;

        mText.Layer = dbText.Layer;

        mText.Color = dbText.Color;

        mText.Attachment = dbText.Justify;

        mText.Width = 0;

        return mText;
    }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new TextFilter();

    /// <inheritdoc />
    protected override GH_AutocadText WrapEntity(CadMText entity) => new GH_AutocadText(entity);

    /// <inheritdoc />
    protected override bool ConvertSupportObject(IEntity entity, out GH_AutocadText supportedGoo)
    {

        if (entity is CadText text)
        {
            var mtext = this.ConvertToMText(text);

            supportedGoo = new GH_AutocadText(mtext);
            return true;
        }
        supportedGoo = null;
        return false;
    }
}
