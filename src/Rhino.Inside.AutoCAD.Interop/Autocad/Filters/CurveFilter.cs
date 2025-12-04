using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class CurveFilter : IFilter
{
    /// <summary>
    /// Constructs a filter that selects entities of which are curves.
    /// </summary>
    public CurveFilter()
    {
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue(-4, "<OR"),
            new TypedValue(0, "ARC,CIRCLE,ELLIPSE,LEADER,LINE,LWPOLYLINE,RAY,SPLINE,XLINE"),
            new TypedValue(-4, "<AND"),
            new TypedValue(0, "POLYLINE"),
            new TypedValue(-4, "&"),
            new TypedValue(70, 16 | 32 | 64),
            new TypedValue(-4, "AND>"),
            new TypedValue(-4, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
