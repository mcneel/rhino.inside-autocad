using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBlockTableRecordRegister"/>
public class BlockTableRecordRegister : RegisterBase<IAutocadBlockTableRecord>, IBlockTableRecordRegister
{
    /// <summary>
    /// Constructs a new <see cref="BlockTableRecordRegister"/>.
    /// </summary>
    public BlockTableRecordRegister(IAutocadDocument document) : base(document)
    { }

    /// <summary>
    /// Populates this <see cref="IBlockTableRecordRegister"/> with <see cref="IAutocadBlockTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public override void Repopulate()
    {
        this.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var database = _document.Database;

            var blockTableId = database.BlockTableId;

            var transactionManager = InteropConverter.Unwrap((ITransactionManager)transactionManagerWrapper);

            var blockTable = (BlockTable)transactionManager.GetObject(InteropConverter.Unwrap((IObjectId)blockTableId), OpenMode.ForRead)!;

            foreach (var objectId in blockTable)
            {
                var blockTableRecord = (BlockTableRecord)transactionManager.GetObject(objectId, OpenMode.ForRead)!;

                var blockTableRecordWrapper = new AutocadBlockTableRecordWrapper(blockTableRecord);

                _objects.Add(blockTableRecordWrapper.Id, blockTableRecordWrapper);
            }

            return true;
        });
    }
}