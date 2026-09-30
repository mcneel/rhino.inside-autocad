namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service to register and retrieve object between Rhino and AutoCAD.
/// This is used by the transient object manager to keep track of which
/// object to update in AutoCAD when a Rhino object is created/modified
/// or deleted.
/// </summary>
/// <remarks>
/// Enumerates the registered entities from the oldest registration to the newest.
/// </remarks>
public interface IObjectRegister : IEnumerable<List<IEntity>>
{
    /// <summary>
    /// The total number of entities registered across every object.
    /// </summary>
    int EntityCount { get; }

    /// <summary>
    /// Tries to get the registered entities for a given Rhino object.
    /// </summary>
    bool TryGetObject(Guid rhinoObjectId, out List<IEntity> entities);

    /// <summary>
    /// Tries to get the object which has been registered the longest, returning false when
    /// the register is empty.
    /// </summary>
    bool TryGetOldest(out Guid rhinoObjectId, out List<IEntity> entities);

    /// <summary>
    /// Registers the given entities for a given Rhino object, replacing any entities already
    /// registered for it and making it the newest registration.
    /// </summary>
    void RegisterObject(Guid rhinoObjectId, List<IEntity> entities);

    /// <summary>
    /// Removes the registered entities for a given Rhino object.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Removes all registered objects that are not in the given set of GUIDs to preserve.
    /// Returns a list of all the removed GUIDs.
    /// </summary>
    HashSet<Guid> RemoveDeletedObjects(HashSet<Guid> guidsToPreserve);
}
