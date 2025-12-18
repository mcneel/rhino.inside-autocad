namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents a wrapper for an AutoCAD Database.
/// </summary>
public interface IDatabase : IDisposable
{
    /// <summary>
    /// Returns the BlockTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId BlockTableId { get; }

    /// <summary>
    /// Returns the LinetypeTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LinetypeTableId { get; }

    /// <summary>
    /// Returns the LayerTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LayerTableId { get; }

    /// <summary>
    /// Returns the LayoutDictionaryId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LayoutDictionaryId { get; }

    /// <summary>
    /// Returns an <see cref="IObjectId"/> from the <see cref="IDatabase"/> using
    /// the provided <paramref name="id"/>. If the id does not exist, or if the id
    /// has been erased <paramref name="isValid"/> is set to false otherwise it is
    /// set to true.
    /// </summary>
    IObjectId GetObjectId(long id, out bool isValid);

    /// <summary>
    /// Opens the <see cref="INamedObjectsDictionary"/> for reading.
    /// </summary>
    INamedObjectsDictionary GetNamedObjectsDictionary();

    /// <summary>+
    /// Returns the <see cref="IObjectId"/> of the ByLayer line type (style).
    /// </summary>
    IObjectId ByLayerLineTypeId { get; }
}
