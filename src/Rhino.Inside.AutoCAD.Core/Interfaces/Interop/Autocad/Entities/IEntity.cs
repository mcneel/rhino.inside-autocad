namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Base interface for all AutoCAD graphical entities that can be displayed in a drawing.
/// </summary>
/// <remarks>
/// Extends <see cref="IDbObject"/> with properties common to all visible geometry including
/// lines, circles, polylines, hatches, and block references. Entities reside in a block
/// table record (model space, paper space, or a block definition) and are assigned to a layer.
/// This interface is the base for geometry-specific interfaces used throughout the
/// Grasshopper component library.
/// </remarks>
/// <seealso cref="IDbObject"/>
/// <seealso cref="IAutocadLayerTableRecord"/>
/// <seealso cref="IAutocadBlockTableRecord"/>
public interface IEntity : IDbObject
{
    /// <summary>
    /// Gets the name of the layer this entity resides on (e.g., "0", "Walls", "Dimensions").
    /// </summary>
    /// <remarks>
    /// The layer controls default visual properties for the entity when set to "ByLayer".
    /// Use <see cref="ILayerRegister"/> to retrieve the full layer definition.
    /// </remarks>
    /// <seealso cref="IAutocadLayerTableRecord"/>
    string LayerName { get; }
}