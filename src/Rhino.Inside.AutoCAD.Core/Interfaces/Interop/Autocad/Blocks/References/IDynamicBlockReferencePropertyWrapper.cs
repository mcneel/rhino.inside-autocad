namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Wraps an Autocad dynamic block reference property.
/// </summary>
public interface IDynamicBlockReferencePropertyWrapper
{
    /// <summary>
    /// The name of the dynamic block reference property.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value of the dynamic block reference property.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Gets a value indicating whether the property is read-only.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Sets the value of the dynamic block reference property. The value is
    /// converted to the appropriate type.
    /// </summary>
    bool SetValue(object propertyValue);
}