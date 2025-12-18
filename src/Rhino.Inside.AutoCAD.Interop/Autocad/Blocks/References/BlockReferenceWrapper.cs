using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBlockReference"/>
public class BlockReferenceWrapper : AutocadEntityWrapper, IBlockReference
{
    private readonly BlockReference _blockReference;

    /// <inheritdoc />
    public IDynamicPropertySet DynamicProperties { get; }

    /// <inheritdoc />
    public double Rotation => _blockReference.Rotation;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockReferenceWrapper(BlockReference blockReference) : base(blockReference)
    {
        _blockReference = blockReference;

        this.Name = blockReference.Name;

        this.BlockTableRecordId = new AutocadObjectId(blockReference.BlockTableRecord);

        this.DynamicProperties = this.PopulateDynamicProperties(blockReference);
    }

    /// <summary>
    /// Obtains the <see cref="DynamicBlockReferenceProperty"/>s of the provided 
    /// <see cref="BlockReference"/>.
    /// </summary>
    private IDynamicPropertySet PopulateDynamicProperties(BlockReference blockReference)
    {
        var blockReferenceProperties = new DynamicPropertySet();

        foreach (DynamicBlockReferenceProperty dynamicProperty in blockReference.DynamicBlockReferencePropertyCollection)
        {
            var wrapped = new DynamicBlockReferencePropertyWrapper(dynamicProperty);

            blockReferenceProperties.Add(wrapped);
        }

        return blockReferenceProperties;
    }

    /// <inheritdoc />
    public void AddCustomProperties(IDynamicPropertySet dynamicProperties)
    {
        foreach (var customProperty in dynamicProperties)
        {
            this.DynamicProperties.Add(customProperty);
        }
    }

    /// <inheritdoc />
    public IEntityCollection GetObjects(ITransactionManager transactionManager)
    {
        var entityCollection = new EntityCollection();
        var blockTableRecord = _blockReference.AnonymousBlockTableRecord;

        var transaction = transactionManager.Unwrap();

        var blockDefinition = transaction.GetObject(blockTableRecord, OpenMode.ForRead) as BlockTableRecord;

        var transform = _blockReference.BlockTransform;

        foreach (var entityId in blockDefinition)
        {
            var entity = transaction.GetObject(entityId, OpenMode.ForRead) as Entity;

            if (entity.Visible == false)
                continue;

            var entityClone = entity.Clone() as Entity;
            entityClone.TransformBy(transform);

            if (entityClone is BlockReference blockReference)
            {
                var wrapper = new BlockReferenceWrapper(blockReference);

                var nestedBlockReferences = wrapper.GetObjects(transactionManager);

                foreach (var nestedEntity in nestedBlockReferences)
                {
                    entityCollection.Add(nestedEntity);
                }
                continue;
            }

            entityCollection.Add(new AutocadEntityWrapper(entityClone));

        }

        return entityCollection;
    }
}