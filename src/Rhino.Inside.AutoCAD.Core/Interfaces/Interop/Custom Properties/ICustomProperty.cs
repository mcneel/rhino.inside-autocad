namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents custom property of the <see cref="IBlockReference"/>.
/// </summary>
public interface ICustomProperty
{
    /// <summary>
    /// The <see cref="ICustomPropertyName"/> of the <see cref="ICustomProperty"/>.
    /// </summary>
    ICustomPropertyName Name { get; }

    /// <summary>
    /// The <see cref="ICustomPropertyValue"/> of the <see cref="ICustomProperty"/>.
    /// </summary>
    ICustomPropertyValue Value { get; }

    /// <summary>
    /// The <see cref="CustomPropertyType"/> of the <see cref="ICustomProperty"/>.
    /// </summary>
    CustomPropertyType Type { get; }

    /// <summary>
    /// Indicates whether the <see cref="ICustomProperty"/> is valid. Technically, this
    /// means that the <see cref="ICustomPropertyName.Type"/> equals the <see cref=
    /// "ICustomPropertyValue.Type"/>.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Sets the provided <see cref="ICustomPropertyValue"/> to the <see cref="ICustomProperty"/>.
    /// </summary>
    /// <param name="value"></param>
    void SetValue(ICustomPropertyValue value);
}