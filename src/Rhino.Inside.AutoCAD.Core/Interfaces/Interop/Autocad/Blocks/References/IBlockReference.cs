namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface representing a wrapped AutoCAD BlockReference
/// </summary>
public interface IBlockReference : IEntity
{
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
    /// Gets the <see cref="IDynamicPropertySet"/>s of this <see cref="IBlockReference"/>.
    /// If the block reference is not open for write the properties on the autocad
    /// internal object are not correctly populated. This method ensures that all
    /// the properties are retrieved correctly.
    /// </summary>
    IDynamicPropertySet GetDynamicProperties(ITransactionManager transactionManager);

    /// <summary>
    /// Gets all the objects contained within this <see cref="IBlockReference"/>.
    /// </summary>
    IEntityCollection GetObjects(ITransactionManager transactionManager);
}