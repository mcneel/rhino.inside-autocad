namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Validates entities for use outside their creating transaction.This validator is ensuring
/// that the entities are safe to be used in the transient manager. It is designed to give
/// constructive feedback to developers if the entities they are creating are not suitable whilst
/// in Debug. However, in production this validator entities which are not valid will just be
/// ignored and not added to the transient manager.
/// </summary>
public interface IEntityValidator
{
    /// <summary>
    /// Validates a collection of entities, ensure they can be used outside the
    /// transaction which created them and returns detailed report.
    /// </summary>
    List<IEntity> ValidateEntitiesForTransientManager(IEnumerable<IEntity> entities, bool silent);
}