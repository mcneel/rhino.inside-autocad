using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Compares <see cref="IEntity"/> objects for equality based on their <see cref="IDbObject.Id"/> property.
/// </summary>
/// <remarks>
/// Two entities are considered equal if their underlying AutoCAD <see cref="IObjectId"/> values match.
/// This comparer is useful for deduplicating entity collections or using entities as dictionary keys
/// where identity is determined by database object ID rather than reference equality.
/// </remarks>
/// <seealso cref="IEntity"/>
/// <seealso cref="IObjectId"/>
public class EntityComparerById : EqualityComparer<IEntity>
{
    /// <inheritdoc/>
    /// <remarks>
    /// Compares the unwrapped <see cref="IObjectId"/> values from each entity.
    /// Returns <c>true</c> if both entities reference the same AutoCAD database object.
    /// </remarks>
    public override bool Equals(IEntity entity, IEntity otherEntity)
    {
        return entity.Id.Unwrap().Equals(otherEntity.Id.Unwrap());
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Generates the hash code from the unwrapped <see cref="IObjectId"/> to ensure
    /// consistency with the <see cref="Equals"/> implementation.
    /// </remarks>
    public override int GetHashCode(IEntity entity)
    {
        return entity.Id.Unwrap().GetHashCode();
    }
}