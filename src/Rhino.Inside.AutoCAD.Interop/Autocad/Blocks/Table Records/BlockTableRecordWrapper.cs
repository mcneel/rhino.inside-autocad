using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps a <see cref="BlockTableRecord"/> from an <see cref="IAutocadDocument"/>.
/// </summary>
/// <remarks>
/// Block table records are located in the <see cref="IBlockTableRecordRepository"/>
/// </remarks>
public class BlockTableRecordWrapper : DbObjectWrapper, IBlockTableRecord
{
    private readonly BlockTableRecord _blockTableRecord;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IObjectIdCollection ObjectIds { get; }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper() : base(new BlockTableRecord())
    {
        _blockTableRecord = (BlockTableRecord)_wrappedValue;

        this.Name = _blockTableRecord.Name;

        this.ObjectIds = new AutocadObjectIdCollection();
    }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper(BlockTableRecord blockTableRecord) : base(blockTableRecord)
    {
        _blockTableRecord = blockTableRecord;

        this.Name = blockTableRecord.Name;

        this.ObjectIds = new AutocadObjectIdCollection();
        foreach (var objectId in blockTableRecord)
        {
            this.ObjectIds.Add(new AutocadObjectId(objectId));
        }
    }

    /// <summary>
    /// Validates the <see cref="IBlockTableRecord"/> object.
    /// </summary>
    protected override bool Validate()
    {
        return base.Validate() && _blockTableRecord is { IsDisposed: false, IsUnloaded: false };
    }

    /// <inheritdoc />
    public IEntityCollection GetObjects(ITransactionManager transactionManager)
    {
        var entityCollection = new EntityCollection();

        var transaction = transactionManager.Unwrap();

        var blockDefinition = transaction.GetObject(_blockTableRecord.Id, OpenMode.ForRead) as BlockTableRecord;

        foreach (var entityId in blockDefinition)
        {
            var entity = transaction.GetObject(entityId, OpenMode.ForRead) as Entity;

            if (entity is BlockReference blockReference)
            {
                var wrapper = new BlockReferenceWrapper(blockReference);

                var nestedBlockReferences = wrapper.GetObjects(transactionManager);

                foreach (var nestedEntity in nestedBlockReferences)
                {
                    entityCollection.Add(nestedEntity);
                }
                continue;
            }

            entityCollection.Add(new AutocadEntityWrapper(entity));

        }

        return entityCollection;
    }
}