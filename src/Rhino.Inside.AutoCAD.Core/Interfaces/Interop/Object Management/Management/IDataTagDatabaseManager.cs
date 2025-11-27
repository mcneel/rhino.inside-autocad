namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a manager type used to control and manage <see cref="IDataTag"/>s
/// and interact with the <see cref="IDatabase"/> to obtain <see cref="IDataTagDatabase"/>s.
/// </summary>
public interface IDataTagDatabaseManager
{
    /// <summary>
    /// The project-wide <see cref="IDataTagDatabase"/> for storing <see cref="IDataTag"/>s
    /// that pertain to the AWI project and have no corresponding <see cref="IDbObject"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="IDataTagDatabase"/> is attached to the <see cref="IDbObject"/>
    /// obtained from <see cref="ITransactionManager.GetModelSpaceBlockTableRecord"/>. Used
    /// for storing project-level <see cref="IDataTag"/>s, such as the name of the
    /// <see cref="ISheetTemplate"/>.
    /// </remarks>
    IProjectWideDataTagDatabase ProjectWideDataTagDatabase { get; }

    /// <summary>
    /// Returns the <see cref="IDataTagDatabase"/> for the given <see cref="IDbObject"/>
    /// or creates a new one if it doesn't exist. 
    /// </summary>
    /// <remarks>
    /// If the <see cref="IDataTagDatabase"/> is created by this call, or if the
    /// <see cref="IDataTag"/>s are modified, the changes are not committed to the
    /// active <see cref="IAutoCadDocument"/>. To commit changes or a new
    /// <see cref="IDataTagDatabase"/> to the <see cref="IAutoCadDocument"/>, clients must
    /// call <see cref="RegisterForCommit"/> so the changes are committed when
    /// <see cref="CommitAll"/> is called.
    /// </remarks>
    IDataTagDatabase GetDatabase(IDbObject dbObject);

    /// <summary>
    /// Registers the <paramref name="dataTagDatabase"/> for commit to the active
    /// <see cref="IAutoCadDocument"/> when <see cref="CommitAll"/> is called, saving any
    /// changes if the <see cref="IDataTagDatabase"/> exists, or adding the
    /// <see cref="IDataTagDatabase"/> and its <see cref="IDataTagRecord"/>s to the
    /// <see cref="IAutoCadDocument"/> if its new. 
    /// </summary>
    /// <remarks>
    /// If the <see cref="IDataTagDatabase"/> is already registered for commit, the
    /// method returns and will not re-register nor duplicate the registration.
    /// </remarks>
    void RegisterForCommit(IDataTagDatabase dataTagDatabase);

    /// <summary>
    /// Commits all changes made to the <see cref="IDataTagDatabase"/>s to the active
    /// <see cref="IAutoCadDocument"/>.
    /// </summary>
    void CommitAll();
}