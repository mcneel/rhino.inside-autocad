using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> which compares two
/// <see cref="ILayer"/>s for equivalence.
/// </summary>
public class LayerComparer : EqualityComparer<ILayer>
{
    /// <summary>
    /// Determines whether the specified <see cref="ILayer"/> objects are equal,
    /// with comparison based on the <see cref="ILayer.Name"/> properties.
    /// </summary>
    /// <remarks>
    /// <see cref="ILayer"/>s are considered equal if their <see cref="ILayer.Name"/>
    /// properties match.
    /// </remarks>
    public override bool Equals(ILayer layer, ILayer otherLayer)
    {
        return string.Equals(layer.Name, otherLayer.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="ILayer"/>.
    /// </summary>
    public override int GetHashCode(ILayer layer)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(layer.Name);
    }
}