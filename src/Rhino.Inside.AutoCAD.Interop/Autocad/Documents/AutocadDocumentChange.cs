using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocumentChange"/>
public class AutocadDocumentChange : IAutocadDocumentChange
{
    private readonly HashSet<IObjectId> _flatObjectList;
    private readonly HashSet<Type> _affectedTypes;
    /// <summary>
    /// A dictionary of change types and their affected objects if there
    /// are any associated with the change.
    /// </summary>
    private readonly Dictionary<ChangeType, List<IDbObject>> _changes;

    /// <inheritdoc />
    public IAutocadDocument Document { get; }

    /// <inheritdoc />
    public bool HasChanges => _changes.Any();

    /// <summary>
    /// Constructs a new <see cref="AutocadDocumentChange"/>, this
    /// will have no changes initially.
    /// </summary>
    public AutocadDocumentChange(IAutocadDocument document)
    {
        _changes = new Dictionary<ChangeType, List<IDbObject>>();

        var comparer = new ObjectIdEqualityComparer();

        _flatObjectList = new HashSet<IObjectId>(comparer);
        _affectedTypes = new HashSet<Type>();

        this.Document = document;
    }

    /// <inheritdoc />
    public void AddChange(ChangeType changeType)
    {
        if (_changes.ContainsKey(changeType) == false)
            _changes[changeType] = new List<IDbObject>();
    }

    /// <inheritdoc />
    public void AddObjectChange(ChangeType changeType, IDbObject affectedObject)
    {
        if (_changes.ContainsKey(changeType) == false)
            _changes[changeType] = new List<IDbObject>();

        _changes[changeType].Add(affectedObject);

        _flatObjectList.Add(affectedObject.Id);

        _affectedTypes.Add(affectedObject.Type);
    }

    /// <inheritdoc />
    public IReadOnlyList<IDbObject> GetAffectedObjects(ChangeType changeType)
    {
        if (_changes.TryGetValue(changeType, out var objects))
            return objects;
        return Array.Empty<DbObjectWrapper>();
    }

    /// <inheritdoc />
    public bool Contains(ChangeType changeType)
    {
        return _changes.ContainsKey(changeType);
    }

    /// <inheritdoc />
    public bool DoesEffectObject(IObjectId objectId)
    {
        return _flatObjectList.Contains(objectId);
    }

    /// <inheritdoc />
    public bool DoesEffectType(Type type)
    {
        return _affectedTypes.Contains(type);
    }

    /// <inheritdoc />
    public IEnumerator<IDbObject> GetEnumerator()
    {
        foreach (var objectList in _changes.Values)
        {
            foreach (var dbObject in objectList)
            {
                yield return dbObject;
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

}

