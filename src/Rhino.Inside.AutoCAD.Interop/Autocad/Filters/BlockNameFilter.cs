using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

public class BlockNameFilter : IBlockNameFilter
{

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Constructs a filter that selects entities of the specified name.
    /// </summary>
    public BlockNameFilter(string name)
    {
        this.Name = name;
    }

    /// <inheritdoc />
    public ISelectionFilter GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new Autodesk.AutoCAD.DatabaseServices.TypedValue((int) DxfCode.BlockName, this.Name.ToUpper()),
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new SelectionFilterWrapper(selectionFilter);
    }
}
