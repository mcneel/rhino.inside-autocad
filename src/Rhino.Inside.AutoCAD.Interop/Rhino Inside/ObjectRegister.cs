using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectRegister"/>
public class ObjectRegister : IObjectRegister
{
    private readonly Dictionary<Guid, List<IEntity>> _objects = [];

    /// <inheritdoc/>
    public bool TryGetObject(Guid rhinoObjectId, out List<IEntity> entities)
    {
        return _objects.TryGetValue(rhinoObjectId, out entities);
    }

    /// <inheritdoc/>
    public void RegisterObject(Guid rhinoObjectId, List<IEntity> entities)
    {
        _objects[rhinoObjectId] = entities;
    }

    /// <inheritdoc/>
    public void RemoveObject(Guid rhinoObjectId)
    {
        if (_objects.ContainsKey(rhinoObjectId))
        {
            _objects.Remove(rhinoObjectId);
        }
    }

    /// <inheritdoc/>
    public HashSet<Guid> RemoveDeletedObjects(HashSet<Guid> guidsToPreserve)
    {
        var keysToRemove = new HashSet<Guid>();
        foreach (var key in _objects.Keys)
        {
            if (guidsToPreserve.Contains(key) == false)
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            this.RemoveObject(key);
        }

        return keysToRemove;
    }

    /// <inheritdoc/>
    public IEnumerator<List<IEntity>> GetEnumerator() => _objects.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}