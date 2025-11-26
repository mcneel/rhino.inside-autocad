namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a database of <see cref="IObjectIdTagRecord"/>s associated with
/// a <see cref="ICeilingInstance"/>.
/// </summary>
/// <remarks>
/// This type is the equivalent to an AutoCAD DBDictionary.
/// </remarks>
public interface IObjectIdTagDatabase : IEnumerable<IObjectIdTagRecord>
{
    /// <summary>
    /// The key of this <see cref="IObjectIdTagDatabase"/> used to obtain it from the
    /// <see cref="IObjectIdTagDatabaseManager"/> in the active database.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns the <see cref="IObjectIdTagRecord"/> by name. If the
    /// <see cref="IObjectIdTagRecord"/> does not exist, it is created and
    /// added to this <see cref="IObjectIdTagDatabase"/>.
    /// </summary>
    IObjectIdTagRecord GetTagRecord(string name);
}