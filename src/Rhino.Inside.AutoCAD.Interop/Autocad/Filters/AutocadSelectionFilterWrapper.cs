using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadSelectionFilterWrapper"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="SelectionFilter"/> which contains DXF-based criteria
/// for filtering entities during selection operations. This wrapper is typically created
/// by filter implementations such as <c>AndFilter</c>, <c>OrFilter</c>, and <c>ObjectByLayerFilter</c>,
/// and is used internally by the GetAutocadObjectsByFilterComponent in the Grasshopper library.
/// </remarks>
/// <seealso cref="IAutocadSelectionFilterWrapper"/>
/// <seealso cref="IObjectFilter"/>
/// <seealso cref="TypedValueWrapper"/>
public class AutocadSelectionFilterWrapper : AutocadWrapperBase<SelectionFilter>, IAutocadSelectionFilterWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadSelectionFilterWrapper"/> class.
    /// </summary>
    /// <param name="value">
    /// The AutoCAD <see cref="SelectionFilter"/> to wrap.
    /// </param>
    public AutocadSelectionFilterWrapper(SelectionFilter value) : base(value)
    {
    }
}
