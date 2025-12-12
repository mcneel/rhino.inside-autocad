using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Leader and MLeader entities.
/// </summary>
public class LeaderFilter : IFilter
{
    /// <summary>
    /// Constructs a filter that selects entities which are Leaders or MLeaders.
    /// </summary>
    public LeaderFilter()
    {
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Operator, "<OR"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Start, "LEADER"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Start, "MLEADER"),
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)DxfCode.Operator, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
