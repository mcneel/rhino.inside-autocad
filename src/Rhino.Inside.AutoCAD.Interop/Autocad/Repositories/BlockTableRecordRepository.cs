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

        this.Repopulate();
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
    public void Repopulate()
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

            this.Repopulate();
        }

        return this.TryGetByName(blockName, out blockTableRecord);
    }

    /// <inheritdoc/>
    public void Register(IBlockTableRecord blockTableRecord)
    {
        if (_blockTableRecords.ContainsKey(blockTableRecord.Name)) return;

        _blockTableRecords.Add(blockTableRecord.Name, blockTableRecord);
    }

    /// <inheritdoc/>
    public bool TryGetById(IObjectId id, out IBlockTableRecord? blockTableRecord)
    {
        blockTableRecord =
            _blockTableRecords.Values.FirstOrDefault(block => block.Id.IsEqualTo(id));

        return blockTableRecord != null;
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
            foreach (var blockTableRecord in _blockTableRecords.Values)
                blockTableRecord.Dispose();

            _disposed = true;
        }
    }
}