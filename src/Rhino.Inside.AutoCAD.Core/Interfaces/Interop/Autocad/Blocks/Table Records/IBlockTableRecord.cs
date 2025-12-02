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
}