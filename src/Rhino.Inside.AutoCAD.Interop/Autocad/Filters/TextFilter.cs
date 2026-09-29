using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Text entities.
/// </summary>
public class TextFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {

        // Taken from the runtime classes rather than written out, so the filter cannot
        // drift from AutoCAD's actual DXF names.
        var textDxfName = RXClass.GetClass(typeof(DBText)).DxfName;
        var mTextDxfName = RXClass.GetClass(typeof(MText)).DxfName;

        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<OR"),
            new TypedValue((int)DxfCode.Start, textDxfName),
            new TypedValue((int)DxfCode.Start, mTextDxfName),
            new TypedValue((int)DxfCode.Operator, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<DBText>() || changedObject.IsOfType<MText>())
                return true;
        }
        return false;
    }
}