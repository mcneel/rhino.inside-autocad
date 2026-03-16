using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadBlockReference"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="BlockReference"/> to expose block instance properties
/// and provide access to dynamic properties, attributes, and contained geometry.
/// Recursively extracts nested block references via <see cref="GetObjects"/>.
/// </remarks>
/// <seealso cref="AutocadEntityWrapper"/>
/// <seealso cref="IBlockTableRecordRegister"/>
public class AutocadBlockReferenceWrapper : AutocadEntityWrapper, IAutocadBlockReference
{
    private readonly BlockReference _blockReference;

    /// <inheritdoc/>
    public double Rotation => _blockReference.Rotation;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IAutocadScale Scale { get; }

    /// <inheritdoc/>
    public Point3d Position { get; }

    /// <inheritdoc/>
    public IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadBlockReferenceWrapper"/>.
    /// </summary>
    /// <param name="blockReference">
    /// The AutoCAD <see cref="BlockReference"/> to wrap.
    /// </param>
    public AutocadBlockReferenceWrapper(BlockReference blockReference) : base(blockReference)
    {
        _blockReference = blockReference;

        this.Name = blockReference.Name;

        this.BlockTableRecordId = new AutocadObjectIdWrapper(blockReference.BlockTableRecord);

        this.Scale = new AutocadScale(_blockReference.ScaleFactors);

        this.Position = _blockReference.Position.ToRhinoPoint3d();
    }

    /// <inheritdoc/>
    public IDynamicPropertySet GetDynamicProperties(ITransactionManager transactionManager)
    {
        var transaction = transactionManager.Unwrap();

        var cadBlockReference = transaction.GetObject(_blockReference.ObjectId,
            OpenMode.ForRead) as BlockReference;

        var blockReferenceProperties = new DynamicPropertySet();

        foreach (DynamicBlockReferenceProperty dynamicProperty in cadBlockReference.DynamicBlockReferencePropertyCollection)
        {
            var wrapped = new DynamicBlockReferencePropertyWrapper(dynamicProperty);

            blockReferenceProperties.Add(wrapped);
        }

        return blockReferenceProperties;
    }

    /// <summary>
    /// Retrieves all attribute references attached to this block reference.
    /// </summary>
    /// <param name="transactionManager">
    /// The transaction manager for database access.
    /// </param>
    /// <returns>
    /// An <see cref="IBlockAttributeSet"/> containing all attribute references.
    /// </returns>
    /// <remarks>
    /// Iterates the block's <see cref="AttributeCollection"/> and wraps each
    /// <see cref="AttributeReference"/> for use in Grasshopper components.
    /// </remarks>
    public IBlockAttributeSet GetAttributes(ITransactionManager transactionManager)
    {
        var transaction = transactionManager.Unwrap();

        var cadBlockReference = transaction.GetObject(_blockReference.ObjectId,
            OpenMode.ForRead) as BlockReference;

        var attributeSet = new BlockAttributeSet();

        foreach (ObjectId attributeId in cadBlockReference.AttributeCollection)
        {
            var attribute = transaction.GetObject(attributeId,
                OpenMode.ForRead) as AttributeReference;

            var wrapped = new AttributeWrapper(attribute);

            attributeSet.Add(wrapped);
        }

        return attributeSet;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Clones and transforms each entity by the block's transform matrix.
    /// Recursively processes nested block references to return flattened geometry.
    /// Skips invisible entities.
    /// </remarks>
    public IEntitySet GetObjects(ITransactionManager transactionManager)
    {
        var entityCollection = new EntitySet();
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
                var wrapper = new AutocadBlockReferenceWrapper(blockReference);

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
