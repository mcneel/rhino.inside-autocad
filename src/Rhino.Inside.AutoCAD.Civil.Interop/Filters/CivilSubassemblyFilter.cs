using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CivilSubassembly = Autodesk.Civil.DatabaseServices.Subassembly;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects Civil 3D Subassembly entities.
/// </summary>
public class CivilSubassemblyFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CivilSubassembly)).DxfName)
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<CivilSubassembly>())
                return true;
        }
        return false;
    }
}
