using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;
using CadObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBlockTableRecordRepository"/>
public class BlockTableRecordRepository : Disposable, IBlockTableRecordRepository
{
    private readonly IAutocadDocument _document;
    private readonly Dictionary<string, IBlockTableRecord> _blockTableRecords;

    /// <inheritdoc/>
    public event EventHandler? BlockTableModified;

    /// <summary>
    /// Constructs a new <see cref="BlockTableRecordRepository"/>.
    /// </summary>
    public BlockTableRecordRepository(IAutocadDocument document)
    {
        _document = document;

        _blockTableRecords = new Dictionary<string, IBlockTableRecord>();

        this.Populate();

        this.SubscribeToModifyEvent();
    }

    /// <summary>
    /// Subscribes to the LayerTable Modified event.
    /// </summary>
    private void SubscribeToModifyEvent()
    {
        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var blockTable =
                (BlockTable)transactionManager.GetObject(_document.Database.BlockTableId.Unwrap(),
                    OpenMode.ForRead);

            blockTable.Modified += this.BlockTable_Modified;

            return true;
        });
    }

    /// <summary>
    /// Unsubscribes from the LayerTable Modified event.
    /// </summary>
    private void UnsubscribeToModifyEvent()
    {
        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var blockTable =
                (BlockTable)transactionManager.GetObject(_document.Database.BlockTableId.Unwrap(),
                    OpenMode.ForRead);

            blockTable.Modified -= this.BlockTable_Modified;

            return true;
        });
    }

    /// <summary>
    /// Handles the LayerTable Modified event.
    /// </summary>
    private void BlockTable_Modified(object sender, EventArgs e)
    {
        this.Populate();
        BlockTableModified?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Clears this repository of all values.
    /// </summary>
    private void Clear()
    {
        _blockTableRecords.Clear();
    }

    /// <inheritdoc/>
    public bool TryGetByName(string name, out IBlockTableRecord? blockTableRecord)
    {
        return _blockTableRecords.TryGetValue(name, out blockTableRecord);
    }

    /// <summary>
    /// Populates this <see cref="IBlockTableRecordRepository"/> with <see cref="IBlockTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    private void Populate()
    {
        this.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var database = _document.Database;

            var blockTableId = database.BlockTableId;

            var transactionManager = transactionManagerWrapper.Unwrap();

            var blockTable = (BlockTable)transactionManager.GetObject(blockTableId.Unwrap(), OpenMode.ForRead)!;

            foreach (var objectId in blockTable)
            {
                var blockTableRecord = (BlockTableRecord)transactionManager.GetObject(objectId, OpenMode.ForRead)!;

                var blockTableRecordWrapper = new BlockTableRecordWrapper(blockTableRecord);

                this.Register(blockTableRecordWrapper);
            }

            return true;
        });
    }

    /// <inheritdoc/>
    public bool TryImportByName(IExternalDatabase externalDatabase, string blockName, out IBlockTableRecord? blockTableRecord)
    {
        if (externalDatabase.TryGetBlockRecord(blockName, out var externalBlockTableRecordWrapper))
        {
            var activeDatabase = _document.Database.Unwrap();

            _ = _document.Transaction(_ =>
            {
                var blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(activeDatabase);

                var externalBlockTableRecord = externalBlockTableRecordWrapper.Unwrap();

                var objectIdCollection =
                    new CadObjectIdCollection { externalBlockTableRecord.Id };

                foreach (var objectId in externalBlockTableRecord)
                {
                    objectIdCollection.Add(objectId);
                }

                using var idMapping = new IdMapping();

                activeDatabase.WblockCloneObjects(objectIdCollection, blockModelSpaceId, idMapping,
                    DuplicateRecordCloning.Replace, false);

                return true;
            });

            this.Populate();
        }

        return this.TryGetByName(blockName, out blockTableRecord);
    }

    /// <inheritdoc/>
    public void Register(IBlockTableRecord blockTableRecord)
    {
        if (_blockTableRecords.ContainsKey(blockTableRecord.Name)) return;

        _blockTableRecords.Add(blockTableRecord.Name, blockTableRecord);
    }

    public IEnumerator<IBlockTableRecord> GetEnumerator() => _blockTableRecords.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Disposes of the <see cref="BlockTableRecordRepository"/> and all contained
    /// <see cref="IBlockTableRecord"/>s.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            this.UnsubscribeToModifyEvent();

            foreach (var blockTableRecord in _blockTableRecords.Values)
                blockTableRecord.Dispose();

            _disposed = true;
        }
    }
}