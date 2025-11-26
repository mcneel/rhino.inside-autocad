using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> which compares two
/// <see cref="IObjectId"/>s for equivalence.
/// </summary>
public class ObjectIdEqualityComparer : EqualityComparer<IObjectId>
{
    /// <summary>
    /// Determines whether the specified <see cref="IObjectId"/> objects are equal,
    /// with comparison based on the <see cref="IObjectId.Value"/> properties.
    /// </summary>
    public override bool Equals(IObjectId id, IObjectId otherId)
    {
        return id.Value.Equals(otherId.Value);
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="IObjectId"/>.
    /// </summary>
    public override int GetHashCode(IObjectId id)
    {
        return id.Unwrap().GetHashCode();
    }
}