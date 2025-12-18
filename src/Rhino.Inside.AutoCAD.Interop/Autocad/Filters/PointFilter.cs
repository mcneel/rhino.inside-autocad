using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;
/// <summary>
/// A filter that selects AutoCAD Point entities.
/// </summary>
public class PointFilter : IFilter
{
    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(0, "POINT")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}