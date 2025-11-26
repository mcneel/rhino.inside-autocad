namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a manager type used to control and manage <see cref="IObjectIdTag"/>s
/// and interact with the <see cref="IDatabase"/> to obtain <see cref="IObjectIdTagDatabase"/>s.
/// </summary>
public interface IObjectIdTagDatabaseManager
{
    /// <summary>
    /// Obtains the <see cref="IObjectIdTagDatabase"/> associated with the <paramref name="key"/>
    /// or creates a new <see cref="IObjectIdTagDatabase"/> if one doesn't exist.
    /// </summary>
    IObjectIdTagDatabase GetDatabase(string key);

    /// <summary>
    /// Commits all changes made to the <see cref="IObjectIdTagDatabase"/>s in this
    /// <see cref="IObjectIdTagDatabaseManager"/> to the active <see cref="IDocument"/>.
    /// </summary>
    void CommitAll();
}