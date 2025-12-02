namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of the <see cref="ICustomProperty"/>s.
/// </summary>
public interface ICustomPropertySet : IEnumerable<ICustomProperty>
{
    /// <summary>
    /// Gets the number of <see cref="ICustomProperty"/>s in the <see cref="ICustomPropertySet"/>.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds the provided <see cref="ICustomProperty"/> in the <see cref=
    /// "ICustomPropertySet"/>. If the <see cref="ICustomProperty.IsValid"/>
    /// is false, it is not added.
    /// </summary>
    void Add(ICustomProperty property);

    /// <summary>
    /// Tries to get the <see cref="ICustomProperty"/> from the <see cref=
    /// "ICustomPropertySet"/>. Returns true if the <paramref name="type"/>
    /// is found. Otherwise, returns false.
    /// </summary>
    bool TryGet(CustomPropertyType type, out ICustomProperty property);
}