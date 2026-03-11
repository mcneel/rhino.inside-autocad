using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ITransactionManager"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="TransactionManager"/> obtained from the active <see cref="Database"/>.
/// Provides methods to access model space and other block table records within a transactional context.
/// All database modifications must occur through this wrapper to ensure proper transaction handling.
/// </remarks>
/// <seealso cref="ITransactionManager"/>
/// <seealso cref="IAutocadDocument"/>
/// <seealso cref="AutocadBlockTableRecordWrapper"/>
public class TransactionManagerWrapper : AutocadWrapperDisposableBase<TransactionManager>, ITransactionManager
{
    private readonly Database _database;
    private readonly DwgVersion _dwgVersion = DwgVersion.Current;

    /// <inheritdoc/>
    public IObjectId BlockTableId { get; }

    /// <inheritdoc/>
    public IObjectId RegAppTableId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionManagerWrapper"/> class.
    /// </summary>
    /// <param name="database">
    /// The AutoCAD <see cref="Database"/> whose <see cref="TransactionManager"/> will be wrapped.
    /// </param>
    /// <remarks>
    /// Captures the database reference for subsequent operations and initializes the
    /// <see cref="BlockTableId"/> from the database's block table.
    /// </remarks>
    public TransactionManagerWrapper(Database database) : base(database.TransactionManager)
    {
        _database = database;
        this.BlockTableId = new AutocadObjectIdWrapper(database.BlockTableId);
        this.RegAppTableId = new AutocadObjectIdWrapper(database.RegAppTableId);
    }

    /// <summary>
    /// Converts a boolean write flag to the corresponding AutoCAD <see cref="OpenMode"/>.
    /// </summary>
    /// <param name="openForWrite">
    /// <c>true</c> for write access; <c>false</c> for read-only access.
    /// </param>
    /// <returns>
    /// <see cref="OpenMode.ForWrite"/> if <paramref name="openForWrite"/> is <c>true</c>;
    /// otherwise, <see cref="OpenMode.ForRead"/>.
    /// </returns>
    private OpenMode GetOpenMode(bool openForWrite) => openForWrite ? OpenMode.ForWrite : OpenMode.ForRead;

    /// <inheritdoc/>
    public IAutocadBlockTableRecord GetModelSpace(bool openForWrite = false)
    {
        var blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(_database);

        var openMode = this.GetOpenMode(openForWrite);

        var blockTableRecord = (BlockTableRecord)blockModelSpaceId.GetObject(openMode);

        return new AutocadBlockTableRecordWrapper(blockTableRecord);
    }

    /// <inheritdoc/>
    public void SaveDatabase(IAutocadDocument document)
    {
        var filePath = _database.Filename;

        if (document.IsReadOnly || string.IsNullOrEmpty(filePath)) return;

        var securityParameters = _database.SecurityParameters;

        _database.SaveAs(filePath, true, _dwgVersion, securityParameters);
    }
}
