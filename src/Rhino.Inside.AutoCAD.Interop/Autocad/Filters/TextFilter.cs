using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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

        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<OR"),
            new TypedValue((int)DxfCode.Start, "TEXT"),
            new TypedValue((int)DxfCode.Start, "MTEXT"),
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