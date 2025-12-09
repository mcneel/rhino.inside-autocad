using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class PointFilter : IFilter
{
    /// <summary>
    /// Constructs a filter that selects entities of which are curves.
    /// </summary>
    public PointFilter()
    {
    }

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