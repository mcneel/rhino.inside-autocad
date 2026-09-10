using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects Civil 3D Corridor entities.
/// </summary>
public class CivilCorridorFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Corridor)).DxfName)
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<Corridor>())
                return true;
        }
        return false;
    }
}
