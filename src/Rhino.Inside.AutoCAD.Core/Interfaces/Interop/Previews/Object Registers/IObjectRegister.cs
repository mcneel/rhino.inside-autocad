namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service to register and retrieve object between Rhino and AutoCAD.
/// This is used by the transient object manager to keep track of which
/// object to update in AutoCAD when a Rhino object is created/modified
/// or deleted.
/// </summary>
/// <remarks>
/// Each registered object is previewed by a single <see cref="IPreviewDrawable"/>, and the
/// register holds the strong reference which keeps it alive while it is registered with
/// AutoCAD as a transient. Enumerates the registered drawables from the oldest registration
/// to the newest.
/// </remarks>
public interface IObjectRegister : IEnumerable<IPreviewDrawable>
{
    /// <summary>
    /// The total number of preview items drawn across every registered drawable.
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// Tries to get the registered drawable for a given Rhino object.
    /// </summary>
    bool TryGetObject(Guid rhinoObjectId, out IPreviewDrawable? drawable);

    /// <summary>
    /// Tries to get the object which has been registered the longest, returning false when
    /// the register is empty.
    /// </summary>
    bool TryGetOldest(out Guid rhinoObjectId, out IPreviewDrawable? drawable);

    /// <summary>
    /// Registers the given drawable for a given Rhino object, replacing any drawable already
    /// registered for it and making it the newest registration.
    /// </summary>
    /// <remarks>
    /// A replaced drawable is only unregistered: the caller erases and disposes it.
    /// </remarks>
    void RegisterObject(Guid rhinoObjectId, IPreviewDrawable drawable);

    /// <summary>
    /// Removes the registered drawable for a given Rhino object, without erasing or
    /// disposing it.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Removes every registered drawable, without erasing or disposing them.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes all registered objects that are not in the given set of GUIDs to preserve.
    /// Returns a list of all the removed GUIDs.
    /// </summary>
    HashSet<Guid> RemoveDeletedObjects(HashSet<Guid> guidsToPreserve);
}
