using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD block reference (an instance of a block definition).
/// </summary>
/// <remarks>
/// Extends <see cref="IEntity"/> with block-specific properties including position, rotation,
/// scale, and access to dynamic properties. Each block reference points to an
/// <see cref="IAutocadBlockTableRecord"/> via <see cref="BlockTableRecordId"/>.
/// Used by the Grasshopper library in <c>AutocadBlockReferenceComponent</c>,
/// <c>GetAutocadBlockReferencesComponent</c>, and <c>CreateAutocadBlockReferenceComponent</c>.
/// </remarks>
/// <seealso cref="IAutocadBlockTableRecord"/>
/// <seealso cref="IEntity"/>
public interface IAutocadBlockReference : IEntity
{
    /// <summary>
    /// Gets the rotation angle in radians.
    /// </summary>
    double Rotation { get; }

    /// <summary>
    /// Gets the block name.
    /// </summary>
    /// <remarks>
    /// Returns the name of the referenced <see cref="IAutocadBlockTableRecord"/>.
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Gets the scale factors for X, Y, and Z axes.
    /// </summary>
    IAutocadScale Scale { get; }

    /// <summary>
    /// Gets the insertion point in Rhino units.
    /// </summary>
    /// <remarks>
    /// Coordinates are automatically converted from AutoCAD units to Rhino units.
    /// </remarks>
    Point3d Position { get; }

    /// <summary>
    /// Gets the <see cref="IObjectId"/> of the parent <see cref="IAutocadBlockTableRecord"/>.
    /// </summary>
    /// <remarks>
    /// Use this ID with <see cref="IBlockTableRecordRegister"/> to retrieve the block definition.
    /// </remarks>
    IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Retrieves the dynamic properties for this block reference.
    /// </summary>
    /// <param name="transactionManager">
    /// The transaction manager for database access.
    /// </param>
    /// <returns>
    /// The <see cref="IDynamicPropertySet"/> containing all dynamic block properties.
    /// </returns>
    /// <remarks>
    /// Opens the block reference for write internally to ensure properties are correctly populated.
    /// Required because AutoCAD does not fully populate dynamic properties in read-only mode.
    /// </remarks>
    IDynamicPropertySet GetDynamicProperties(ITransactionManager transactionManager);

    /// <summary>
    /// Retrieves all entities contained within this block reference.
    /// </summary>
    /// <param name="transactionManager">
    /// The transaction manager for database access.
    /// </param>
    /// <returns>
    /// An <see cref="IEntitySet"/> containing the block's geometry.
    /// </returns>
    IEntitySet GetObjects(ITransactionManager transactionManager);
}
