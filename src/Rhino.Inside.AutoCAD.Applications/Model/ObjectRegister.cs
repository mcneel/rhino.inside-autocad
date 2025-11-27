using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IObjectRegister"/>
public class ObjectRegister : IObjectRegister
{
    private readonly Dictionary<Guid, List<IEntity>> _objects = [];

    /// <inheritdoc/>
    public bool TryGetObjectId(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        return _objects.TryGetValue(rhinoObject.Id, out entities);
    }

    /// <inheritdoc/>
    public void RegisterObjectId(RhinoObject rhinoObject, List<IEntity> entities)
    {
        _objects[rhinoObject.Id] = entities;
    }
}