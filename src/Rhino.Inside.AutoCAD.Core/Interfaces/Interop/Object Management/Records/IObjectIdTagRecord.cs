namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a record (collection) of <see cref="IObjectIdTag"/>s.
/// </summary>
/// <remarks>
/// This type is the equivalent to an AutoCAD XRecord.
/// </remarks>
public interface IObjectIdTagRecord : IEnumerable<IObjectIdTag>
{
    /// <summary>
    /// The name of this <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    /// <remarks>
    /// This value is used as a key when added to the <see cref="IObjectIdTagDatabase"/>.
    /// </remarks>
    string Key { get; }

    /// <summary>
    /// Adds an existing <see cref="IObjectIdTag"/> to this
    /// <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    void AddExisting(IObjectIdTag objectIdTag);

    /// <summary>
    /// Registers the <paramref name="taggedObjectId"/> with this
    /// <see cref="IObjectIdTagRecord"/>. If the extracted <see cref="IObjectIdTag"/>
    /// already exists in this <see cref="IObjectIdTagRecord"/> the
    /// <paramref name="taggedObjectId"/> is ignored.
    /// </summary>
    void Register(ITaggedObjectId taggedObjectId);

    /// <summary>
    /// Registers the <paramref name="entity"/> with this
    /// <see cref="IObjectIdTagRecord"/>. If the extracted <see cref="IObjectIdTag"/>
    /// already exists in this <see cref="IObjectIdTagRecord"/> the
    /// <paramref name="entity"/> is ignored.
    /// </summary>
    void Register(IEntity entity);

    /// <summary>
    /// Registers the <paramref name="objectIdTags"/> with this <see cref=
    /// "IObjectIdTagRecord"/>. If the extracted <see cref="IObjectIdTag"/>s
    /// already exist in this <see cref="IObjectIdTagRecord"/> they are ignored.
    /// </summary>
    void Register(IObjectIdTagCollection objectIdTags);

    /// <summary>
    /// De-registers the <paramref name="tag"/> from this <see cref="IObjectIdTagRecord"/>.
    /// </summary>
    void Deregister(IObjectIdTag tag);

    /// <summary>
    /// Clears this <see cref="IObjectIdTagRecord"/> of all <see cref="IObjectIdTag"/>s
    /// and deletes all tagged objects from the <see cref="IDatabase"/>.
    /// </summary>
    void Erase(IObjectEraser eraser);

    /// <summary>
    /// Returns the list of registered <see cref="IObjectIdTag"/>s that are stored in the
    /// active <see cref="IDocument"/>.
    /// </summary>
    IList<IObjectIdTag> GetRegisteredTags();
}