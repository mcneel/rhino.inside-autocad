namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines a filter for selecting AutoCAD entities based on specific criteria.
/// </summary>
/// <remarks>
/// Filters encapsulate AutoCAD selection set filter logic using DXF group codes and values.
/// Common implementations include type filters (lines, circles), layer filters, and
/// compound filters (AND/OR combinations). Used by Grasshopper components such as
/// GetAutocadObjectsByFilterComponent, ObjectByLayerFilterComponent, and the various
/// converter components to query entities from a document.
/// </remarks>
/// <seealso cref="IAutocadSelectionFilterWrapper"/>
/// <seealso cref="ITypedValueWrapper"/>
public interface IObjectFilter
{
    /// <summary>
    /// Creates the <see cref="IAutocadSelectionFilterWrapper"/> representing this filter's criteria.
    /// </summary>
    /// <returns>
    /// An <see cref="IAutocadSelectionFilterWrapper"/> containing the DXF filter criteria
    /// that can be passed to AutoCAD's selection methods.
    /// </returns>
    /// <remarks>
    /// The returned wrapper contains typed values with DXF group codes. For example,
    /// entity type filters use group code 0 with values like "LINE" or "CIRCLE".
    /// </remarks>
    IAutocadSelectionFilterWrapper GetSelectionFilter();
}