namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a value of the <see cref="ICustomProperty"/>.
/// </summary>
public interface ICustomPropertyValue
{
    /// <summary>
    /// Type of the <see cref="ICustomProperty"/>.
    /// </summary>
    CustomPropertyType Type { get; }

    /// <summary>
    /// Value of the <see cref="ICustomProperty"/>.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Determines whether the provided <see cref="ICustomPropertyValue"/> is equal
    /// to this <see cref="ICustomPropertyValue"/>.
    /// </summary>
    bool IsEqualTo(ICustomPropertyValue other);
}