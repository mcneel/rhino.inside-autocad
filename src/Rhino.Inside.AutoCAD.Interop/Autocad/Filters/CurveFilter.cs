using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that selects AutoCAD Curve entities.
/// </summary>
public class CurveFilter : IObjectFilter
{
    /// <summary>
    /// The POLYLINE flag bits this filter matches: polygon mesh (16), closed in the N
    /// direction (32) and polyface mesh (64).
    /// </summary>
    /// <remarks>
    /// Preserved as it was found. Note that these are the mesh flags rather than the 3D
    /// polyline flag (8), so this branch admits meshes to a curve filter, which looks
    /// unintended but is long standing behaviour and is not changed here.
    /// </remarks>
    /// <remarks>Carried on DXF group code 70, <see cref="DxfCode.Int16"/>.</remarks>
    private const int MeshPolylineFlags = 16 | 32 | 64;

    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var curveDxfNames = DxfName.Of(typeof(Arc), typeof(Circle), typeof(Ellipse),
            typeof(Leader), typeof(Line), typeof(Polyline), typeof(Ray), typeof(Spline),
            typeof(Xline));

        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.Operator, "<OR"),
            new TypedValue((int)DxfCode.Start, curveDxfNames),
            new TypedValue((int)DxfCode.Operator, "<AND"),
            new TypedValue((int)DxfCode.Start, DxfName.Of<Polyline2d>()),
            new TypedValue((int)DxfCode.Operator, "&"),
            new TypedValue((int)DxfCode.Int16, MeshPolylineFlags),
            new TypedValue((int)DxfCode.Operator, "AND>"),
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
            if (changedObject.IsOfType<Curve>())
                return true;
        }
        return false;
    }
}