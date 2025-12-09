namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a manager type used to control and manage <see cref="ITypedValue"/>s
/// and interact with the <see cref="IDatabase"/> to obtain <see cref="IXRecordDictionary"/>s.
/// </summary>
public interface IXRecordDictionaryManager
{
    /// <summary>
    /// The project-wide <see cref="IXRecordDictionary"/> for storing <see cref="IDataTag"/>s
    /// that pertain to the AWI project and have no corresponding <see cref="IDbObject"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="IXRecordDictionary"/> is attached to the <see cref="IDbObject"/>
    /// obtained from <see cref="ITransactionManager.GetModelSpaceBlockTableRecord"/>. Used
    /// for storing project-level <see cref="IDataTag"/>s, such as the name of the
    /// <see cref="ISheetTemplate"/>.
    /// </remarks>
    IProjectWideXRecordDictionary ProjectWideXRecordDictionary { get; }

    /// <summary>
    /// Returns the <see cref="IXRecordDictionary"/> for the given <see cref="IDbObject"/>
    /// or creates a new one if it doesn't exist. 
    /// </summary>
    /// <remarks>
    /// If the <see cref="IXRecordDictionary"/> is created by this call, or if the
    /// <see cref="ITypedValue"/>s are modified, the changes are not committed to the
    /// active <see cref="IAutocadDocument"/>. To commit changes or a new
    /// <see cref="IXRecordDictionary"/> to the <see cref="IAutocadDocument"/>, clients must
    /// call <see cref="RegisterForCommit"/> so the changes are committed when
    /// <see cref="CommitAll"/> is called.
    /// </remarks>
    IXRecordDictionary GetDatabase(IDbObject dbObject);

    /// <summary>
    /// Registers the <paramref name="xrecordDictionary"/> for commit to the active
    /// <see cref="IAutocadDocument"/> when <see cref="CommitAll"/> is called, saving any
    /// changes if the <see cref="IXRecordDictionary"/> exists, or adding the
    /// <see cref="IXRecordDictionary"/> and its <see cref="IXRecord"/>s to the
    /// <see cref="IAutocadDocument"/> if its new. 
    /// </summary>
    /// <remarks>
    /// If the <see cref="IXRecordDictionary"/> is already registered for commit, the
    /// method returns and will not re-register nor duplicate the registration.
    /// </remarks>
    void RegisterForCommit(IXRecordDictionary xrecordDictionary);

    /// <summary>
    /// Commits all changes made to the <see cref="IXRecordDictionary"/>s to the active
    /// <see cref="IAutocadDocument"/>.
    /// </summary>
    void CommitAll();
}