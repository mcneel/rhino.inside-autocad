using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD block instances.
/// </summary>
public class Param_AutocadBlockReference : GH_PersistentParam<GH_AutocadBlockReference>, IReferenceParam
{
    private const string _singularPromptMessage = "Select a Block Reference";
    private const string _pluralPromptMessage = "Select Block References";

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("b4e8f0a2-5d9c-7e3f-0b2a-8f5c4d7e9a3f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadBlockReference;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadCurve"/> class.
    /// </summary>
    public Param_AutocadBlockReference()
        : base(new GH_InstanceDescription("AutoCAD Block Reference", "BlockRef",
            "A Block Reference in AutoCAD", "Params", "AutoCAD"))
    { }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Singular(ref GH_AutocadBlockReference value)
    {
        var picker = new AutocadObjectPicker();

        var filter = new BlockReferenceFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entity = picker.PickObject(selectionFilter, _singularPromptMessage);

        if (entity.Unwrap() is BlockReference typedEntity)
        {
            var wrapper = new BlockReferenceWrapper(typedEntity);

            value = new GH_AutocadBlockReference(wrapper);

            return GH_GetterResult.success;
        }

        value = default;
        return GH_GetterResult.cancel;
    }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Plural(ref List<GH_AutocadBlockReference> values)
    {
        var picker = new AutocadObjectPicker();

        var filter = new BlockReferenceFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var entities = picker.PickObjects(selectionFilter, _pluralPromptMessage);

        foreach (var entity in entities)
        {
            if (entity is BlockReference typedEntity)
            {
                var wrapper = new BlockReferenceWrapper(typedEntity);

                values.Add(new GH_AutocadBlockReference(wrapper));
            }
        }

        return GH_GetterResult.success;
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {

        foreach (var blockRef in m_data.AllData(true).OfType<GH_AutocadBlockReference>())
        {
            if (change.DoesEffectObject(blockRef.Value.Id))
                return true;

            if (change.DoesEffectObject(blockRef.Value.BlockTableRecordId))
                return true;
        }

        return false;
    }
}
