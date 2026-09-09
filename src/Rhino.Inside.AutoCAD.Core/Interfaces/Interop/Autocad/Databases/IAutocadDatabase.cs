namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapper for an AutoCAD Database, providing access to core symbol tables and dictionaries.
/// </summary>
/// <remarks>
/// This interface abstracts the underlying AutoCAD database object, enabling access to essential
/// table identifiers such as blocks, layers, linetypes, and layouts. Wrappers of this kind never
/// own the database they wrap - a document's database belongs to AutoCAD - so this interface is
/// deliberately not <see cref="IDisposable"/>.
/// </remarks>
/// <seealso cref="IObjectId"/>
public interface IAutocadDatabase
{
    /// <summary>
    /// Gets the <see cref="IObjectId"/> for the Block Table in this database.
    /// </summary>
    /// <remarks>
    /// The Block Table contains all block definitions (block table records) in the drawing,
    /// including model space, paper space, and user-defined blocks.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    IObjectId BlockTableId { get; }

    /// <summary>
    /// Gets the <see cref="IObjectId"/> for the Linetype Table in this database.
    /// </summary>
    /// <remarks>
    /// The Linetype Table stores all linetype definitions available in the drawing,
    /// such as continuous, dashed, and custom linetypes.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    IObjectId LinetypeTableId { get; }

    /// <summary>
    /// Gets the <see cref="IObjectId"/> for the Layer Table in this database.
    /// </summary>
    /// <remarks>
    /// The Layer Table contains all layer definitions in the drawing, controlling
    /// visibility, color, linetype, and other properties for entities on each layer.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    IObjectId LayerTableId { get; }

    /// <summary>
    /// Gets the <see cref="IObjectId"/> for the Layout Dictionary in this database.
    /// </summary>
    /// <remarks>
    /// The Layout Dictionary contains all layout definitions in the drawing,
    /// including model space and any paper space layouts with their viewport configurations.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    IObjectId LayoutDictionaryId { get; }
}
