using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents a  custom filter built by the user. 
/// </summary>
public class CustomFilter : IObjectFilter
{
    /// <summary>
    /// The list of typed values that define the filter criteria. Each <see
    /// cref="ITypedValueWrapper"/> in the list represents a single criterion for filtering
    /// entities, where the type code specifies the property to filter on and the value
    /// specifies the desired value for that property. The filter will select all entities
    /// that match all of the specified criteria.
    /// </summary>
    public List<ITypedValueWrapper> TypedValues { get; }

    /// <summary>
    /// Constructs a filter that selects entities based on the specified layer Name.
    /// </summary>
    public CustomFilter(List<ITypedValueWrapper> typedValues)
    {
        this.TypedValues = typedValues;
    }

    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var selectionFilter = new SelectionFilter(this.TypedValues
            .OfType<TypedValueWrapper>()
            .Select(wrapper => wrapper.Unwrap())
            .ToArray());

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }
}
