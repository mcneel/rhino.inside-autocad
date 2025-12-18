namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An object which models the changes which have occurred to a <see cref="IAutocadDocument"/>.
/// These change are compiled during a command's execution and are used in the document change
/// event to query the changes once the command has completed.
/// </summary>
public interface IAutocadDocumentChange : IEnumerable<IDbObject>
{
    /// <summary>
    /// The affected <see cref="IAutocadDocument"/>.
    /// </summary>
    IAutocadDocument Document { get; }

    /// <summary>
    /// Boolean which indicates if there are any changes.
    /// </summary>
    bool HasChanges { get; }

    /// <summary>
    /// Adds a change of the specified <paramref name="changeType"/> which does not
    /// have any associated affected objects.
    /// </summary>
    void AddChange(ChangeType changeType);

    /// <summary>
    /// Adds a change of the specified <paramref name="changeType"/> with the associated
    /// objects affected by the change.
    /// </summary>
    void AddObjectChange(ChangeType changeType, IDbObject affectedObject);

    /// <summary>
    /// Returns a read-only list of affected objects for the specified <paramref
    /// name="changeType"/>, if there are any otherwise an empty list. If the
    /// change type does not exist, an empty list is also returned.
    /// </summary>
    IReadOnlyList<IDbObject> GetAffectedObjects(ChangeType changeType);

    /// <summary>
    /// If the change contains the specified <paramref name="changeType"/> this returns
    /// true, otherwise false.
    /// </summary>
    bool Contains(ChangeType changeType);

    /// <summary>
    /// Checks if the change effects the specified <paramref name="objectId"/>.
    /// </summary>
    bool DoesEffectObject(IObjectId objectId);

    /// <summary>
    /// Checks if the change effects any objects of the specified <paramref name="type"/>.
    /// This is particularly useful when checking for if repositories need to be updated
    /// as a result of the change.
    /// </summary>
    bool DoesEffectType(Type type);
}