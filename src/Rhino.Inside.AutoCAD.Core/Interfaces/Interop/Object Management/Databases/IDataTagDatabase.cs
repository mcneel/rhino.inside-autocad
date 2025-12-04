namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A database for extracting, storing and reading
/// <see cref="IDataTagRecord"/>s and their <see cref="IDataTag"/>s.
/// <see cref="IDataTagDatabase"/> are attached to <see cref="IDbObject"/>s
/// in the active <see cref="IAutocadDocument"/> using its extensible storage.
/// </summary>
public interface IDataTagDatabase : IEnumerable<IDataTagRecord>
{
    /// <summary>
    /// The <see cref="IDbObject"/> this <see cref="IDataTagDatabase"/> is attached to
    /// in the <see cref="IAutocadDocument"/>.
    /// </summary>
    IDbObject DbObjectOwner { get; }

    /// <summary>
    /// Returns the <see cref="IDataTagRecord"/> that exists at the given key or creates
    /// a <see cref="IDataTagRecord"/> in this <see cref="IDataTagDatabase"/> if it doesn't
    /// and returns the new <see cref="IDataTagRecord"/>.
    /// </summary>
    IDataTagRecord GetRecord(string key);

    /// <summary>
    /// Returns true and assigns the <see cref="IDataTagRecord"/> if it exists.
    /// otherwise returns false.
    /// </summary>
    bool TryGetRecord(string key, out IDataTagRecord? dataTagRecord);
}