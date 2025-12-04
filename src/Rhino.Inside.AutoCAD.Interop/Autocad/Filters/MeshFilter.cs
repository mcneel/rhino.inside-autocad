using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class MeshFilter : IFilter
{
    /// <summary>
    /// Constructs a filter that selects entities which are polyface meshes.
    /// </summary>
    public MeshFilter()
    {
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue(-4, "<AND"),
            new TypedValue((int)DxfCode.Start, "POLYLINE"),
            new TypedValue(70, 64),
            new TypedValue(-4, "AND>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
