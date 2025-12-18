namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface representing a wrapped Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
/// class.
/// </summary>
public interface IBlockTableRecord : IDbObject
{
    /// <summary>
    /// The name of the <see cref="IBlockTableRecord"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The collection of <see cref="IObjectId"/>s contained in the <see cref="IBlockTableRecord"/>.
    /// </summary>
    public IObjectIdCollection ObjectIds { get; }

    /// <summary>
    /// Gets all the objects contained within this <see cref="IBlockTableRecord"/>.
    /// </summary>
    IEntityCollection GetObjects(ITransactionManager transactionManager);
}