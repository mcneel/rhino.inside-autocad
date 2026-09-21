using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;
/// <summary>
/// A filter that selects AutoCAD Mesh entities.
/// </summary>
public class MeshFilter : IObjectFilter
{
    /// <summary>
    /// The POLYLINE flag bit marking a polyface mesh.
    /// </summary>
    /// <remarks>Carried on DXF group code 70, <see cref="DxfCode.Int16"/>.</remarks>
    private const int PolyfaceMeshFlag = 64;

    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<AND"),
            new TypedValue((int)DxfCode.Start, DxfName.Of<PolyFaceMesh>()),
            new TypedValue((int)DxfCode.Int16, PolyfaceMeshFlag),
            new TypedValue((int)DxfCode.Operator, "AND>")
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