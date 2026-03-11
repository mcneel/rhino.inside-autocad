namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD block definition (BlockTableRecord) containing geometry and attributes.
/// </summary>
/// <remarks>
/// Wraps an AutoCAD BlockTableRecord to expose block definition properties and entity access.
/// Block table records include user-defined blocks, model space ("*Model_Space"), and paper
/// space layouts ("*Paper_Space"). Used by Grasshopper components including
/// AutocadBlockTableRecordComponent, GetAutocadBlockTableRecordsComponent, and
/// AutocadExtractBlockGeometryComponent.
/// </remarks>
/// <seealso cref="IBlockTableRecordRegister"/>
/// <seealso cref="IAutocadBlockReference"/>
/// <seealso cref="INamedDbObject"/>
public interface IAutocadBlockTableRecord : INamedDbObject
{
    /// <summary>
    /// Gets the collection of <see cref="IObjectId"/>s for all entities contained in this block.
    /// </summary>
    /// <remarks>
    /// This provides lightweight access to entity identifiers without opening the entities.
    /// Use <see cref="GetObjects"/> to retrieve the actual entity wrappers.
    /// </remarks>
    /// <seealso cref="GetObjects"/>
    public IObjectIdCollection ObjectIds { get; }

    /// <summary>
    /// Gets the <see cref="IObjectId"/> of this block's extension dictionary.
    /// </summary>
    /// <remarks>
    /// The extension dictionary stores custom data associated with the block definition.
    /// Returns a null ObjectId if no extension dictionary exists.
    /// </remarks>
    /// <seealso cref="IAutocadDictionary"/>
    IObjectId ExtensionDictionary { get; }

    /// <summary>
    /// Retrieves all entities contained within this block definition.
    /// </summary>
    /// <param name="transactionManager">
    /// The <see cref="ITransactionManager"/> used to open the entities for read access.
    /// </param>
    /// <returns>
    /// An <see cref="IEntitySet"/> containing wrapped entities from this block.
    /// </returns>
    /// <remarks>
    /// Used by AutocadExtractBlockGeometryComponent to extract geometry from block definitions.
    /// The returned entities are opened within the provided transaction context.
    /// </remarks>
    /// <seealso cref="ObjectIds"/>
    IEntitySet GetObjects(ITransactionManager transactionManager);
}

