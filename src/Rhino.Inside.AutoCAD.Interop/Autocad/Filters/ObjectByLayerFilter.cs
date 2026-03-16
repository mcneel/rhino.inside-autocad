using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents a filter that selects AutoCAD entities based on their layer. 
/// </summary>
public class ObjectByLayerFilter : IObjectFilter
{
    /// <summary>
    /// The name of the layer that the filter will use to select entities.
    /// </summary>
    public string LayerName { get; }

    /// <summary>
    /// Constructs a filter that selects entities based on the specified layer Name.
    /// </summary>
    public ObjectByLayerFilter(string layerName)
    {
        this.LayerName = layerName;
    }

    /// <summary>
    /// Constructs a filter that selects entities based on the specified layer. The filter
    /// will select all entities that are on the same layer as the provided <see
    /// cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    public ObjectByLayerFilter(IAutocadLayerTableRecord layer)
    {
        this.LayerName = layer.Name;
    }

    /// <inheritdoc />
    public IAutocadSelectionFilterWrapper GetSelectionFilter()
    {
        var filterCriteria = new[]
        {
            new TypedValue((int)DxfCode.LayerName, this.LayerName)
        };

        var selectionFilter = new SelectionFilter(filterCriteria);

        return new AutocadSelectionFilterWrapper(selectionFilter);
    }
}