namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface representing a wrapped AutoCAD BlockReference
/// </summary>
public interface IBlockReference : IEntity
{
    /// <summary>
    /// The <see cref="IDynamicPropertySet"/>s of this <see cref="IBlockReference"/>.
    /// </summary>
    IDynamicPropertySet DynamicProperties { get; }

    /// <summary>
    /// The rotation of this <see cref="IBlockReference"/> in radians.
    /// </summary>
    double Rotation { get; }

    /// <summary>
    /// The name of the <see cref="IBlockReference"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of the <see cref="IBlockTableRecord"/> which
    /// this <see cref="IBlockReference"/> is an instance of.
    /// </summary>
    IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Adds <see cref="IDynamicBlockReferencePropertyWrapper"/>s to the <see
    /// cref="DynamicProperties"/> from the <see cref="IDynamicPropertySet"/>.
    /// </summary>
    void AddCustomProperties(IDynamicPropertySet customProperties);

    /// <summary>
    /// Gets all the objects contained within this <see cref="IBlockReference"/>.
    /// </summary>
    IEntityCollection GetObjects(ITransactionManager transactionManager);
}