using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class CurveFilter : IFilter
{
    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(-4, "<OR"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(0, "ARC,CIRCLE,ELLIPSE,LEADER,LINE,LWPOLYLINE,RAY,SPLINE,XLINE"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(-4, "<AND"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(0, "POLYLINE"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(-4, "&"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(70, 16 | 32 | 64),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(-4, "AND>"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue(-4, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
