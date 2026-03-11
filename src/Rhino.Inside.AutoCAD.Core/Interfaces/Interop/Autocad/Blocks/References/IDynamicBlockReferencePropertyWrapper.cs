namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a dynamic property of an AutoCAD dynamic block reference.
/// </summary>
/// <remarks>
/// Dynamic block properties allow parametric control of block geometry, such as stretch
/// distances, visibility states, and lookup values. Properties can be constrained to
/// specific allowed values. Used by Grasshopper components including
/// AutocadDynamicPropertiesComponent and SetAutocadDynamicPropertiesComponent.
/// </remarks>
/// <seealso cref="IAutocadBlockReference"/>
/// <seealso cref="DynamicPropertyTypeCode"/>
public interface IDynamicBlockReferencePropertyWrapper
{
    /// <summary>
    /// Gets the name of this dynamic property (e.g., "Distance1", "Visibility1").
    /// </summary>
    /// <remarks>
    /// Property names are defined in the block definition and are case-sensitive.
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Gets the current value of this dynamic property.
    /// </summary>
    /// <remarks>
    /// The value type depends on <see cref="TypeCode"/>. Common types include
    /// <see cref="double"/> for distances, <see cref="string"/> for visibility states,
    /// and <see cref="short"/> for lookup indices.
    /// </remarks>
    /// <seealso cref="TypeCode"/>
    /// <seealso cref="SetValue"/>
    object Value { get; }

    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    /// <remarks>
    /// Read-only properties are typically driven by other parameters or constraints
    /// and cannot be modified directly via <see cref="SetValue"/>.
    /// </remarks>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets the list of allowed values for this property, if constrained.
    /// </summary>
    /// <remarks>
    /// Returns an empty array if the property accepts any value within its type.
    /// For visibility states, contains the available state names. For lookup
    /// properties, contains the valid lookup values.
    /// </remarks>
    public object[] AllowedValues { get; }

    /// <summary>
    /// Gets the <see cref="DynamicPropertyTypeCode"/> indicating the data type of this property.
    /// </summary>
    /// <remarks>
    /// Use this to determine how to interpret and set the <see cref="Value"/> property.
    /// </remarks>
    /// <seealso cref="Value"/>
    public DynamicPropertyTypeCode TypeCode { get; }

    /// <summary>
    /// Sets the value of this dynamic property.
    /// </summary>
    /// <param name="propertyValue">
    /// The new value to assign. Will be converted to the appropriate type based on <see cref="TypeCode"/>.
    /// </param>
    /// <param name="transactionManager">
    /// The <see cref="ITransactionManager"/> for the database operation.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value was set successfully; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Returns <c>false</c> if the property is read-only, the value is not in
    /// <see cref="AllowedValues"/> (when constrained), or type conversion fails.
    /// </remarks>
    /// <seealso cref="Value"/>
    /// <seealso cref="AllowedValues"/>
    bool SetValue(object propertyValue, ITransactionManager transactionManager);
}