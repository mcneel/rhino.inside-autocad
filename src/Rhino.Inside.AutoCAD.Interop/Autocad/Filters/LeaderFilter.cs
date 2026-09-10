using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Leader and MLeader entities.
/// </summary>
public class LeaderFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<OR"),
            new TypedValue((int)DxfCode.Start, "LEADER"),
            new TypedValue((int)DxfCode.Start, "MLEADER"),
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
            if (changedObject.IsOfType<Leader>() || changedObject.IsOfType<MLeader>())
                return true;
        }
        return false;
    }
}