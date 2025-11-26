namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a record (collection) of <see cref="IDataTag"/>s. A <see cref="IDataTagRecord"/>
/// is assigned to the Extensions Dictionary of a AutoCAD API DBObject. It therefore represents
/// a Xrecord/ResultBuffer, where each <see cref="IDataTag"/> is a typed value added to the buffer.
/// </summary>
public interface IDataTagRecord : IEnumerable<IDataTag>
{
    /// <summary>
    /// The key name of this <see cref="IDataTagRecord"/>.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns true if this <see cref="IDataTagRecord"/> contains any
    /// <see cref="IDataTag"/>s with the <paramref name="groupCode"/>,
    /// otherwise returns false.
    /// </summary>
    bool ContainsKey(GroupCodeValue groupCode);

    /// <summary>
    /// Returns true if this <see cref="IDataTagRecord"/> contains any <see cref=
    /// "IDataTag"/>s with the same <paramref name="value"/> within the <paramref
    /// name="groupCode"/>, otherwise returns false.
    /// </summary>
    bool ContainsValue(GroupCodeValue groupCode, object value);

    /// <summary>
    /// Returns the list of <see cref="IDataTag"/>s that match the given
    /// <paramref name="groupCode"/> or an empty list if no matches are
    /// found.
    /// </summary>
    IList<IDataTag> GetAt(GroupCodeValue groupCode);

    /// <summary>
    /// Returns the value from the first <see cref="IDataTag"/> matching the given
    /// <paramref name="groupCode"/>. If the <paramref name="groupCode"/> does not
    /// exist, the default of <typeparamref name="TValue"/> is returned. Use
    /// this method when this <see cref="IDataTagRecord"/> is guaranteed to have
    /// only one <see cref="IDataTag"/> of the given <paramref name="groupCode"/>.
    /// </summary>
    TValue GetFirstValueAt<TValue>(GroupCodeValue groupCode);

    /// <summary>
    /// Replaces an existing <see cref="IDataTag"/> in this <see cref="IDataTagRecord"/>
    /// with the given <paramref name="groupCode"/> and <paramref name="value"/>.
    /// </summary>
    void Replace(GroupCodeValue groupCode, object value);

    /// <summary>
    /// Adds a new <see cref="IDataTag"/> to this <see cref="IDataTagRecord"/>
    /// with the given <paramref name="key"/> and <paramref name="value"/>.
    /// </summary>
    void Add(GroupCodeValue key, object value);

    /// <summary>
    /// Clears all the <see cref="IDataTag"/>s of the <see cref="IDataTagRecord"/>.
    /// </summary>
    void Clear();

    /// <summary>
    /// Identifies if the <see cref="IDataTagRecord"/> contains any <see cref="IDataTag"/>s.
    /// </summary>
    bool IsEmpty();
}