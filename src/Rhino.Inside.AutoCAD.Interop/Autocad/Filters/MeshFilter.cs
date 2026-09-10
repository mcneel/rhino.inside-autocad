using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;
/// <summary>
/// A filter that selects AutoCAD Mesh entities.
/// </summary>
public class MeshFilter : IObjectFilter
{
    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue(-4, "<AND"),
            new TypedValue((int)DxfCode.Start, "POLYLINE"),
            new TypedValue(70, 64),
            new TypedValue(-4, "AND>")
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }

    /// <inheritdoc />
    public bool IsAffectedByChange(IAutocadDocumentChange change)
    {
        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<PolygonMesh>() || changedObject.IsOfType<PolyFaceMesh>())
                return true;
        }
        return false;
    }
}