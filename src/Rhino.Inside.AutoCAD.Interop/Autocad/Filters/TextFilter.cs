using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class TextFilter : IFilter
{
    /// <summary>
    /// Constructs a filter that selects entities which are Text Objects.
    /// </summary>
    public TextFilter()
    {
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {

        var filterCriteria = new[]
        {
            new  Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Operator, "<OR"),
            new  Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Start, "TEXT"),
            new  Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Start, "MTEXT"),
            new  Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Operator, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
