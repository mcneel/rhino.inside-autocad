using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Dimension entities.
/// </summary>
public class DimensionFilter : IFilter
{
    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Start, "DIMENSION")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
