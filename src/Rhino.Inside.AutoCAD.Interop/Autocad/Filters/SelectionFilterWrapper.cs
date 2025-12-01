using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ISelectionFilter"/>
public class SelectionFilterWrapper : WrapperBase<SelectionFilter>, ISelectionFilter
{
    /// <summary>
    /// Constructs a new <see cref="ISelectionFilter"/>. From the Autocad
    /// <see cref="SelectionFilter"/>
    /// </summary>
    public SelectionFilterWrapper(SelectionFilter value) : base(value)
    {
    }
}