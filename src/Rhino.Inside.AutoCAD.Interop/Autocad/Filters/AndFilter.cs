using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A filter that combines two other filters using a logical AND operation. Only objects
/// that satisfy both filters will be selected.
/// </summary>
public class AndFilter : IObjectFilter
{

    /// <summary>
    /// Filter A is the first filter in the logical AND operation. It can be any filter
    /// that implements the <see cref="IObjectFilter"/> interface.
    /// </summary>
    public IObjectFilter FilterA { get; }

    /// <summary>
    /// Filter B is combined with Filter A using a logical AND operation.  It can be
    /// any filter that implements the <see cref="IObjectFilter"/> interface.
    /// </summary>
    public IObjectFilter FilterB { get; }

    /// <summary>
    /// Constructs a new <see cref="AndFilter"/> by combining two filters with a logical AND operation.
    /// </summary>
    public AndFilter(IObjectFilter filterA, IObjectFilter filterB)
    {
        this.FilterA = filterA;
        this.FilterB = filterB;
    }

    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterA = this.FilterA.GetSelectionFilter();
        var filterB = this.FilterB.GetSelectionFilter();

        var filterCriteria = new TypedValue[] { new(-4, "<AND") }
            .Concat(filterA.Unwrap().GetFilter())
            .Concat(filterB.Unwrap().GetFilter())
            .Append(new TypedValue(-4, "AND>"))
            .ToArray();

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }
}