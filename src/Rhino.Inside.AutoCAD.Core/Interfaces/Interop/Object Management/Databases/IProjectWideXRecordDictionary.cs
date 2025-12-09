namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The project-wide <see cref="IXRecordDictionary"/> for storing <see cref="ITypedValue"/>s
/// that pertain to the AWI project and have no corresponding <see cref="IDbObject"/>.
/// </summary>
/// <remarks>
/// The returned <see cref="IXRecordDictionary"/> is attached to the <see cref="IDbObject"/>
/// obtained from <see cref="ITransactionManager.GetModelSpaceBlockTableRecord"/>. Used
/// for storing project-level <see cref="ITypedValue"/>s, such as the name of the
/// <see cref="ISheetTemplate"/>.
/// </remarks>
public interface IProjectWideXRecordDictionary
{
    /// <summary>
    /// The project-wide <see cref="IXRecordDictionary"/> for storing <see cref="ITypedValue"/>s.
    /// </summary>
    IXRecordDictionary Database { get; }

    /// <summary>
    /// The <see cref="IDbObject"/> the <see cref="IXRecordDictionary"/> is attached to
    /// in the <see cref="IAutocadDocument"/>, This is forwarded from the <see cref="Database"/>.
    /// </summary>
    IDbObject DbObjectOwner { get; }

    /// <summary>
    /// Returns the <see cref="IXRecord"/> that exists at the given key or creates
    /// a <see cref="IXRecord"/> in this <see cref="IXRecordDictionary"/> if it doesn't
    /// and returns the new <see cref="IXRecord"/>. This is forwarded from the <see
    /// cref="Database"/>.
    /// </summary>
    IXRecord GetRecord(string key);

    /// <summary>
    /// Returns true and assigns the <see cref="IDataTagRecord"/> if it exists.
    /// otherwise returns false. This is forwarded from the <see cref="Database"/>.
    /// </summary>
    bool TryGetRecord(string key, out IXRecord? dataTagRecord);
}