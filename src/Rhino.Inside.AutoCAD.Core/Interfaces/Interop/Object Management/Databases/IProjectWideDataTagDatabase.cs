namespace Rhino.Inside.AutoCAD.Core.Interfaces;

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
public interface IProjectWideDataTagDatabase
{
    /// <summary>
    /// The project-wide <see cref="IDataTagDatabase"/> for storing <see cref="IDataTag"/>s.
    /// </summary>
    IDataTagDatabase Database { get; }

    /// <summary>
    /// The <see cref="IDbObject"/> the <see cref="IDataTagDatabase"/> is attached to
    /// in the <see cref="IAutocadDocument"/>, This is forwarded from the <see cref="Database"/>.
    /// </summary>
    IDbObject DbObjectOwner { get; }

    /// <summary>
    /// Returns the <see cref="IDataTagRecord"/> that exists at the given key or creates
    /// a <see cref="IDataTagRecord"/> in this <see cref="IDataTagDatabase"/> if it doesn't
    /// and returns the new <see cref="IDataTagRecord"/>. This is forwarded from the <see
    /// cref="Database"/>.
    /// </summary>
    IDataTagRecord GetRecord(string key);

    /// <summary>
    /// Returns true and assigns the <see cref="IDataTagRecord"/> if it exists.
    /// otherwise returns false. This is forwarded from the <see cref="Database"/>.
    /// </summary>
    bool TryGetRecord(string key, out IDataTagRecord? dataTagRecord);
}