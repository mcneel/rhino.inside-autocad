using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadBlockTableRecord"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="BlockTableRecord"/> (block definition) to expose
/// block properties and contained entity IDs. Managed by <see cref="IBlockTableRecordRegister"/>.
/// Used by the Grasshopper library in <c>AutocadBlockTableRecordComponent</c>,
/// <c>GetAutocadBlockTableRecordsComponent</c>, and <c>AutocadExtractBlockGeometryComponent</c>.
/// </remarks>
/// <seealso cref="IBlockTableRecordRegister"/>
/// <seealso cref="AutocadBlockReferenceWrapper"/>
public class AutocadBlockTableRecordWrapper : AutocadDbObjectWrapper, IAutocadBlockTableRecord
{
    private readonly BlockTableRecord _blockTableRecord;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectIdCollection ObjectIds { get; }

    /// <inheritdoc/>
    public IObjectId ExtensionDictionary { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadBlockTableRecordWrapper"/>.
    /// </summary>
    /// <param name="blockTableRecord">
    /// The AutoCAD <see cref="BlockTableRecord"/> to wrap.
    /// </param>
    public AutocadBlockTableRecordWrapper(BlockTableRecord blockTableRecord) : base(blockTableRecord)
    {
        _blockTableRecord = blockTableRecord;

        this.Name = blockTableRecord.Name;

        this.ObjectIds = new AutocadObjectIdCollection();
        foreach (var objectId in blockTableRecord)
        {
            this.ObjectIds.Add(new AutocadObjectIdWrapper(objectId));
        }

        this.ExtensionDictionary = new AutocadObjectIdWrapper(blockTableRecord.ExtensionDictionary);
    }

    /// <summary>
    /// Validates that the block table record is usable.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the record is valid, not disposed, and not unloaded; otherwise <c>false</c>.
    /// </returns>
    protected override bool Validate()
    {
        return base.Validate() && _blockTableRecord is { IsDisposed: false, IsUnloaded: false };
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Recursively extracts geometry from nested block references.
    /// Returns a flattened collection of all entities within the block definition.
    /// </remarks>
    public IEntitySet GetObjects(ITransactionManager transactionManager)
    {
        var entityCollection = new EntitySet();

        var transaction = transactionManager.Unwrap();

        var blockDefinition = transaction.GetObject(_blockTableRecord.Id, OpenMode.ForRead) as BlockTableRecord;

        foreach (var entityId in blockDefinition)
        {
            var entity = transaction.GetObject(entityId, OpenMode.ForRead) as Entity;

            if (entity is BlockReference blockReference)
            {
                var wrapper = new AutocadBlockReferenceWrapper(blockReference);

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
