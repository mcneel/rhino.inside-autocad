using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> which compares two
/// <see cref="IAutocadLayerTableRecord"/>s for equivalence.
/// </summary>
public class LayerComparer : EqualityComparer<IAutocadLayerTableRecord>
{
    /// <summary>
    /// Determines whether the specified <see cref="IAutocadLayerTableRecord"/> objects are equal,
    /// with comparison based on the <see cref="IAutocadLayerTableRecord.Name"/> properties.
    /// </summary>
    /// <remarks>
    /// <see cref="IAutocadLayerTableRecord"/>s are considered equal if their <see cref="IAutocadLayerTableRecord.Name"/>
    /// properties match.
    /// </remarks>
    public override bool Equals(IAutocadLayerTableRecord layer, IAutocadLayerTableRecord otherLayer)
    {
        return string.Equals(layer.Name, otherLayer.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    public override int GetHashCode(IAutocadLayerTableRecord layer)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(layer.Name);
    }
}