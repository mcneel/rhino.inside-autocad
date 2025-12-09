namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a record (collection) of <see cref="ITypedValue"/>s. A <see cref="IXRecord"/>
/// is assigned to the Extensions Dictionary of a AutoCAD API DBObject. It therefore represents
/// a Xrecord/ResultBuffer, where each <see cref="ITypedValue"/> is a typed value added to the buffer.
/// </summary>
public interface IXRecord : IEnumerable<ITypedValue>
{
    /// <summary>
    /// The key name of this <see cref="IXRecord"/>.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns true if this <see cref="IXRecord"/> contains any
    /// <see cref="ITypedValue"/>s with the <paramref name="groupCode"/>,
    /// otherwise returns false.
    /// </summary>
    bool ContainsKey(GroupCodeValue groupCode);

    /// <summary>
    /// Returns true if this <see cref="IXRecord"/> contains any <see cref=
    /// "ITypedValue"/>s with the same <paramref name="value"/> within the <paramref
    /// name="groupCode"/>, otherwise returns false.
    /// </summary>
    bool ContainsValue(GroupCodeValue groupCode, object value);

    /// <summary>
    /// Returns the list of <see cref="ITypedValue"/>s that match the given
    /// <paramref name="groupCode"/> or an empty list if no matches are
    /// found.
    /// </summary>
    IList<ITypedValue> GetAt(GroupCodeValue groupCode);

    /// <summary>
    /// Returns the value from the first <see cref="ITypedValue"/> matching the given
    /// <paramref name="groupCode"/>. If the <paramref name="groupCode"/> does not
    /// exist, the default of <typeparamref name="TValue"/> is returned. Use
    /// this method when this <see cref="IXRecord"/> is guaranteed to have
    /// only one <see cref="ITypedValue"/> of the given <paramref name="groupCode"/>.
    /// </summary>
    TValue GetFirstValueAt<TValue>(GroupCodeValue groupCode);

    /// <summary>
    /// Replaces an existing <see cref="ITypedValue"/> in this <see cref="IXRecord"/>
    /// with the given <paramref name="groupCode"/> and <paramref name="value"/>.
    /// </summary>
    void Replace(GroupCodeValue groupCode, object value);

    /// <summary>
    /// Adds a new <see cref="ITypedValue"/> to this <see cref="IXRecord"/>
    /// with the given <paramref name="key"/> and <paramref name="value"/>.
    /// </summary>
    void Add(GroupCodeValue key, object value);

    /// <summary>
    /// Clears all the <see cref="ITypedValue"/>s of the <see cref="IXRecord"/>.
    /// </summary>
    void Clear();

    /// <summary>
    /// Identifies if the <see cref="IXRecord"/> contains any <see cref="ITypedValue"/>s.
    /// </summary>
    bool IsEmpty();
}