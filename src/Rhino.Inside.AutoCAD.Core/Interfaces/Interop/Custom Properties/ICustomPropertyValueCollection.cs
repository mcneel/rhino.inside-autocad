namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of the <see cref="ICustomPropertyValue"/>s obtained
/// from some specific <see cref="ICustomPropertyProvider"/>. It's used to store
/// values of <see cref="ICustomProperty"/>s in the objects where the properties
/// will be obtained from.
/// </summary>
public interface ICustomPropertyValueCollection : IEnumerable<ICustomPropertyValue>
{
    /// <summary>
    /// Returns the <see cref="ICustomPropertyValue"/> from the <see 
    /// cref="ICustomPropertyValueCollection"/>. If the <paramref name="type"/> is not 
    /// found, returns default instance <see cref="ICustomPropertyValue"/>.
    /// </summary>
    ICustomPropertyValue this[CustomPropertyType type] { get; }

    /// <summary>
    /// Adds the provided <see cref="ICustomPropertyValue"/> in the <see cref=
    /// "ICustomPropertyValueCollection"/>.
    /// </summary>
    void Add(ICustomPropertyValue value);

    /// <summary>
    /// Adds the provided <see cref="ICustomPropertyValue"/>s in the <see cref=
    /// "ICustomPropertyValueCollection"/>.
    /// </summary>
    void Add(IList<ICustomPropertyValue> values);
}