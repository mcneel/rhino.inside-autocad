using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IEntityValidator"/>
public class EntityValidator : IEntityValidator
{
    /// <summary>
    /// Validates that an entity is suitable for use with TransientManager.
    /// </summary>
    private bool ValidateForTransientManagerSilent(IEntity entity)
    {
        var cadEntity = entity.Unwrap();

        return cadEntity != null && !cadEntity.IsDisposed && !cadEntity.IsErased
               && (cadEntity.ObjectId.IsNull || cadEntity is not { IsTransactionResident: false, IsReadEnabled: false, IsWriteEnabled: false })
               && (cadEntity.ObjectId.IsNull || cadEntity.Database == null);
    }

    /// <summary>
    /// Validates that an entity is suitable for use with TransientManager.
    /// This will give detailed error messages for debugging purposes.
    /// </summary>
    private bool ValidateForTransientManagerLoud(IEntity entity, out string message)
    {
        message = "This is a DEBUG only error, If you see this it means some" +
                  " autocad geometry has been created badly and we need to fix it. " +
                  "In production, the offending geometry will just not be previewed. \n";

        var cadEntity = entity.Unwrap();

        if (cadEntity == null)
        {
            message = "Entity is null.";
            return false;
        }

        if (cadEntity.IsDisposed)
        {
            message = $"Entity of type {cadEntity.GetType().Name} has been disposed. " +
                      "To fix: Ensure you are not disposing the entity.";
            return false;
        }

        if (cadEntity.IsErased)
        {
            message =
                $"Entity of type {cadEntity.GetType().Name} is erased (Handle: {cadEntity.Handle}). " +
                "To fix, clone the entity before erasing the original: " +
                "var clone = entity.Clone() as Entity;";

            return false;
        }

        if (!cadEntity.ObjectId.IsNull && !cadEntity.IsTransactionResident && !cadEntity.IsReadEnabled && !cadEntity.IsWriteEnabled)
        {
            message =
               $"Entity of type {cadEntity.GetType().Name} is in an invalid state " +
                $"(Handle: {cadEntity.Handle}, ObjectId: {cadEntity.ObjectId}). " +
                "This typically happens when a database-resident entity is returned" +
                " after its transaction has been committed. " +
                "e.g. it is a zombie object. To fix, either: " +
                "1) Clone the entity before the transaction commits, or " +
                "2) Create the entity without adding it to the database.";

            return false;
        }

        // Check if it's database-resident (not ideal for transients)
        if (!cadEntity.ObjectId.IsNull && cadEntity.Database != null)
        {
            message =
                 $"Entity of type {cadEntity.GetType().Name} is database-resident " +
                $"(Handle: {cadEntity.Handle}, BlockName: {cadEntity.BlockName}). " +
                "To fix, it requires non-database-resident entities. " +
                "Use entity.Clone() to create a free-standing copy.";

            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates a collection of entities, ensure they can be used outside the transaction
    /// which created them. This method returns a filtered list of valid entities.
    /// If there are invalid detailed errors are collected for invalid entities an error with
    /// a report will be thrown.
    /// </summary>
    private List<IEntity> ValidateEntitiesForTransientManagerLoud(IEnumerable<IEntity> entities)
    {
        var validationResult = new EntityValidationResult();

        var validEntities = new List<IEntity>();

        foreach (var entity in entities)
        {

            if (this.ValidateForTransientManagerLoud(entity, out var error))
            {
                validEntities.Add(entity);
                continue;
            }

            var cadEntity = entity.Unwrap();

            validationResult.AddError(new EntityValidationError
            {
                EntityType = cadEntity?.GetType().Name ?? "null",
                Handle = cadEntity?.Handle.ToString() ?? "N/A",
                Message = error
            });
        }

        if (validationResult.IsValid == false)
            throw new InvalidOperationException(validationResult.ToString());

        return validEntities;
    }

    /// <summary>
    /// Validates a collection of entities, ensure they can be used outside the transaction
    /// which created them. This method returns a filtered list of valid entities.
    /// </summary>
    private List<IEntity> ValidateEntitiesForTransientManagerSilent(IEnumerable<IEntity> entities)
    {
        var validEntities = new List<IEntity>();
        foreach (var entity in entities)
        {
            if (this.ValidateForTransientManagerSilent(entity))
            {
                validEntities.Add(entity);
            }
        }

        return validEntities;
    }

    /// <inheritdoc />
    public List<IEntity> ValidateEntitiesForTransientManager(IEnumerable<IEntity> entities, bool silent)
    {
        return silent
            ? this.ValidateEntitiesForTransientManagerSilent(entities)
            : this.ValidateEntitiesForTransientManagerLoud(entities);
    }
}