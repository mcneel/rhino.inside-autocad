using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Curve entities.
/// </summary>
public class CurveFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
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

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<Curve>())
                return true;
        }
        return false;
    }
}