using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An <see cref="IEqualityComparer{IEntity}"/> which compares two
/// <see cref="IEntity"/>s for equivalence.
/// </summary>
public class EntityComparer : EqualityComparer<IEntity>
{
    /// <summary>
    /// Determines whether the specified <see cref="IEntity"/> objects are equal, with
    /// comparison based on the <see cref="IEntity.Id"/> properties.
    /// </summary>
    /// <remarks>
    /// <see cref="IEntity"/>s are considered equal if their <see cref="IEntity.Id"/>
    /// properties match.
    /// </remarks>
    public override bool Equals(IEntity entity, IEntity otherEntity)
    {
        return entity.Id.Unwrap().Equals(otherEntity.Id.Unwrap());
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="IEntity"/>.
    /// </summary>
    public override int GetHashCode(IEntity entity)
    {
        return entity.Id.Unwrap().GetHashCode();
    }
}