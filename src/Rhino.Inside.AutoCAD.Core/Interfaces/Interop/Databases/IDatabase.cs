namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents a wrapper for an AutoCAD Database.
/// </summary>
public interface IDatabase : IDisposable
{
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

    /// <summary>
    /// Returns the <see cref="IObjectId"/> of the ByLayer line type (style).
    /// </summary>
    IObjectId ByLayerLineTypeId { get; }
}
