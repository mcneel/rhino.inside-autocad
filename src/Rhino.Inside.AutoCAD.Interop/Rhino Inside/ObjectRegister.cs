using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectRegister"/>
/// <remarks>
/// Registration order is tracked alongside the lookup so the oldest object can be found
/// when a preview server has to make room for a newer one.
/// </remarks>
public class ObjectRegister : IObjectRegister
{
    private readonly Dictionary<Guid, IPreviewDrawable> _objects = [];
    private readonly LinkedList<Guid> _registrationOrder = [];
    private readonly Dictionary<Guid, LinkedListNode<Guid>> _registrationNodes = [];

    /// <inheritdoc/>
    public int ItemCount { get; private set; }

    /// <inheritdoc/>
    public bool TryGetObject(Guid rhinoObjectId, out IPreviewDrawable? drawable)
    {
        return _objects.TryGetValue(rhinoObjectId, out drawable);
    }

    /// <inheritdoc/>
    public bool TryGetOldest(out Guid rhinoObjectId, out IPreviewDrawable? drawable)
    {
        var oldestNode = _registrationOrder.First;

        if (oldestNode == null)
        {
            rhinoObjectId = Guid.Empty;
            drawable = null;
            return false;
        }

        rhinoObjectId = oldestNode.Value;
        drawable = _objects[rhinoObjectId];
        return true;
    }

    /// <inheritdoc/>
    public void RegisterObject(Guid rhinoObjectId, IPreviewDrawable drawable)
    {
        this.RemoveObject(rhinoObjectId);

        _objects[rhinoObjectId] = drawable;
        _registrationNodes[rhinoObjectId] = _registrationOrder.AddLast(rhinoObjectId);
        this.ItemCount += drawable.ItemCount;
    }

    /// <inheritdoc/>
    public void RemoveObject(Guid rhinoObjectId)
    {
        if (_objects.TryGetValue(rhinoObjectId, out var drawable))
        {
            this.ItemCount -= drawable.ItemCount;
            _objects.Remove(rhinoObjectId);
        }

        if (_registrationNodes.TryGetValue(rhinoObjectId, out var node))
        {
            _registrationOrder.Remove(node);
            _registrationNodes.Remove(rhinoObjectId);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _objects.Clear();
        _registrationOrder.Clear();
        _registrationNodes.Clear();
        this.ItemCount = 0;
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
    /// <remarks>
    /// Enumerates from the oldest registration to the newest.
    /// </remarks>
    public IEnumerator<IPreviewDrawable> GetEnumerator() =>
        _registrationOrder.Select(rhinoObjectId => _objects[rhinoObjectId]).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
