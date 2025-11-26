using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ITransactionManager"/>
public class TransactionManagerWrapper : WrapperDisposableBase<TransactionManager>, ITransactionManager
{
    private Database _database;

    /// <summary>
    /// Constructs a new <see cref="TransactionManagerWrapper"/>.
    /// </summary>
    public TransactionManagerWrapper(Database database) : base(database.TransactionManager)
    {
        _database = database;
    }

    /// <summary>
    /// Returns the <see cref="OpenMode"/> via the <paramref name="openForWrite"/> flag.
    /// </summary>
    private OpenMode GetOpenMode(bool openForWrite) => openForWrite ? OpenMode.ForWrite : OpenMode.ForRead;

    /// <inheritdoc/>
    public IBlockTableRecord GetModelSpaceBlockTableRecord(bool openForWrite = false)
    {
        var blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(_database);

        var openMode = this.GetOpenMode(openForWrite);

        var blockTableRecord = (BlockTableRecord)blockModelSpaceId.GetObject(openMode);

        return new BlockTableRecordWrapper(blockTableRecord);
    }
}