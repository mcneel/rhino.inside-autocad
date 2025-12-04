using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> which compares two
/// <see cref="IAutocadLayer"/>s for equivalence.
/// </summary>
public class LayerComparer : EqualityComparer<IAutocadLayer>
{
    /// <summary>
    /// Determines whether the specified <see cref="IAutocadLayer"/> objects are equal,
    /// with comparison based on the <see cref="IAutocadLayer.Name"/> properties.
    /// </summary>
    /// <remarks>
    /// <see cref="IAutocadLayer"/>s are considered equal if their <see cref="IAutocadLayer.Name"/>
    /// properties match.
    /// </remarks>
    public override bool Equals(IAutocadLayer layer, IAutocadLayer otherLayer)
    {
        return string.Equals(layer.Name, otherLayer.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="IAutocadLayer"/>.
    /// </summary>
    public override int GetHashCode(IAutocadLayer layer)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(layer.Name);
    }
}