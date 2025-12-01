using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectRegister"/>
public class ObjectRegister : IObjectRegister
{
    private readonly Dictionary<Guid, List<IEntity>> _objects = [];

    /// <inheritdoc/>
    public bool TryGetObject(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        return _objects.TryGetValue(rhinoObject.Id, out entities);
    }

    /// <inheritdoc/>
    public void RegisterObject(RhinoObject rhinoObject, List<IEntity> entities)
    {
        _objects[rhinoObject.Id] = entities;
    }

    /// <inheritdoc/>
    public void RemoveObject(RhinoObject rhinoObject)
    {
        _objects.Remove(rhinoObject.Id);
    }

    /// <inheritdoc/>
    public IEnumerator<List<IEntity>> GetEnumerator() => _objects.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}