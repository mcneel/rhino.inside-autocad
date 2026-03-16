namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Wraps an AutoCAD SelectionFilter containing DXF-based filter criteria.
/// </summary>
/// <remarks>
/// This marker interface abstracts the AutoCAD SelectionFilter class, which uses arrays of
/// TypedValue objects to define entity selection criteria. Implementations contain the
/// actual filter data used by AutoCAD's selection set methods. Created by
/// <see cref="IObjectFilter.GetSelectionFilter"/> and used internally by filter components
/// to query entities from a document.
/// </remarks>
/// <seealso cref="IObjectFilter"/>
/// <seealso cref="ITypedValueWrapper"/>
public interface IAutocadSelectionFilterWrapper
{
}