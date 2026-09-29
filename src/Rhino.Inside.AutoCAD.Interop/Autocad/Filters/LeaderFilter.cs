using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Leader and MLeader entities.
/// </summary>
public class LeaderFilter : IObjectFilter
{
    /// <inheritdoc />
    /// <remarks>
    /// The DXF names are taken from the runtime classes rather than written out, because
    /// an entity's DXF name is not its class name or its command name: an MLeader is
    /// MULTILEADER in DXF, so the literal "MLEADER" silently matched nothing and no
    /// multileader could ever be picked.
    /// </remarks>
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var leaderDxfName = RXClass.GetClass(typeof(Leader)).DxfName;
        var mLeaderDxfName = RXClass.GetClass(typeof(MLeader)).DxfName;

        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<OR"),
            new TypedValue((int)DxfCode.Start, leaderDxfName),
            new TypedValue((int)DxfCode.Start, mLeaderDxfName),
            new TypedValue((int)DxfCode.Operator, "OR>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<Leader>() || changedObject.IsOfType<MLeader>())
                return true;
        }
        return false;
    }
}