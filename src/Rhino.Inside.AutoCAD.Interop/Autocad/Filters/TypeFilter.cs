using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class TypeFilter : ITypeFilter
{
    /// <inheritdoc />
    public string TypeName { get; }

    /// <summary>
    /// Constructs a filter that selects entities of the specified type.
    /// </summary>
    /// <param name="typeName"></param>
    public TypeFilter(string typeName)
    {
        this.TypeName = typeName;
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int) DxfCode.Start, this.TypeName.ToUpper()),
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}