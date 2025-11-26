using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectIdTagRecord"/>
public class ObjectIdTagRecord : IObjectIdTagRecord
{
    private readonly Dictionary<long, IObjectIdTag> _objectTags;
    private readonly Dictionary<long, IObjectIdTag> _deRegisteredTags;

    /// <inheritdoc/>
    public string Key { get; }

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTagRecord"/>.
    /// </summary>
    public ObjectIdTagRecord(string name)
    {
        _objectTags = new Dictionary<long, IObjectIdTag>();
        _deRegisteredTags = new Dictionary<long, IObjectIdTag>();

        this.Key = name;
    }

    /// <inheritdoc/>
    public void AddExisting(IObjectIdTag objectIdTag)
    {
        _objectTags[objectIdTag.Id] = objectIdTag;
    }

    /// <inheritdoc/>
    public void Register(ITaggedObjectId taggedObjectId)
    {
        var objectTag = taggedObjectId.GetTag();

        var key = objectTag.Id;

        if (_objectTags.ContainsKey(key))
            return;

        _objectTags.Add(key, objectTag);
    }

    /// <inheritdoc/>
    public void Register(IEntity entity)
    {
        var id = entity.Id;

        var key = id.Value;

        if (_objectTags.ContainsKey(key)) return;

        var objectIdTag = new ObjectIdTag(id);

        _objectTags.Add(key, objectIdTag);
    }

    /// <inheritdoc/>
    public void Register(IObjectIdTagCollection objectIdTags)
    {
        foreach (var objectIdTag in objectIdTags)
        {
            var key = objectIdTag.Id;

            if (_objectTags.ContainsKey(key)) continue;

            _objectTags.Add(key, objectIdTag);
        }
    }

    /// <inheritdoc/>
    public void Deregister(IObjectIdTag tag)
    {
        var key = tag.Id;

        if (_objectTags.ContainsKey(key)) return;

        _deRegisteredTags[key] = tag;
    }

    /// <inheritdoc/>
    public void Erase(IObjectEraser eraser)
    {
        var document = eraser.Document;

        var database = document.Database;

        _ = document.Transaction(_ =>
        {
            foreach (var objectTag in _objectTags.Values)
            {
                var objectId = database.GetObjectId(objectTag.Id, out var isValidObjectId);

                if (isValidObjectId == false) continue;

                var objectIdUnwrapped = objectId.Unwrap();

                var dbObject = objectIdUnwrapped.GetObject(OpenMode.ForWrite, true, true);

                var dbObjectWrapper = new DbObject(dbObject);

                eraser.Erase(dbObjectWrapper);
            }

            return true;
        });

        _objectTags.Clear();
    }

    /// <inheritdoc/>
    public IList<IObjectIdTag> GetRegisteredTags()
    {
        return _objectTags.Values.Except(_deRegisteredTags.Values).ToList();
    }

    /// <inheritdoc/>
    public IEnumerator<IObjectIdTag> GetEnumerator()
    {
        foreach (var objectTag in _objectTags.Values)
        {
            yield return objectTag;
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}