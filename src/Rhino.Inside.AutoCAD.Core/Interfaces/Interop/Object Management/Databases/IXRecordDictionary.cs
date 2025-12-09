namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A database for extracting, storing and reading
/// <see cref="IXRecord"/>s and their <see cref="ITypedValue"/>s.
/// <see cref="IXRecordDictionary"/> are attached to <see cref="IDbObject"/>s
/// in the active <see cref="IAutocadDocument"/> using its extensible storage.
/// </summary>
public interface IXRecordDictionary : IEnumerable<IXRecord>
{
    /// <summary>
    /// The <see cref="IDbObject"/> this <see cref="IXRecordDictionary"/> is attached to
    /// in the <see cref="IAutocadDocument"/>.
    /// </summary>
    IDbObject DbObjectOwner { get; }

    /// <summary>
    /// Returns the <see cref="IXRecord"/> that exists at the given key or creates
    /// a <see cref="IXRecord"/> in this <see cref="IXRecordDictionary"/> if it doesn't
    /// and returns the new <see cref="IXRecord"/>.
    /// </summary>
    IXRecord GetRecord(string key);

    /// <summary>
    /// Returns true and assigns the <see cref="IXRecord"/> if it exists.
    /// otherwise returns false.
    /// </summary>
    bool TryGetRecord(string key, out IXRecord? dataTagRecord);
}