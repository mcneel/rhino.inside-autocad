using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Compares <see cref="IObjectId"/> instances for equality based on their <see cref="IObjectId.Value"/> property.
/// </summary>
/// <remarks>
/// Two object IDs are considered equal if their underlying pointer values match.
/// This comparer is useful for deduplicating object ID collections or using them as dictionary keys
/// where identity is determined by the database pointer value rather than reference equality.
/// </remarks>
/// <seealso cref="IObjectId"/>
/// <seealso cref="EntityComparerById"/>
public class ObjectIdEqualityComparer : EqualityComparer<IObjectId>
{
    /// <inheritdoc/>
    /// <remarks>
    /// Compares the <see cref="IObjectId.Value"/> properties of each object ID.
    /// Returns <c>true</c> if both IDs reference the same AutoCAD database object.
    /// </remarks>
    public override bool Equals(IObjectId id, IObjectId otherId)
    {
        return id.Value.Equals(otherId.Value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Generates the hash code from the unwrapped <see cref="IObjectId"/> to ensure
    /// consistency with the <see cref="Equals"/> implementation.
    /// </remarks>
    public override int GetHashCode(IObjectId id)
    {
        return id.Unwrap().GetHashCode();
    }
}