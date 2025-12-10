using Rhino.Geometry;

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
    /// The Origin point of the <see cref="IBlockTableRecord"/>.
    /// </summary>
    public Point3d Origin { get; }

    /// <summary>
    /// The collection of <see cref="IObjectId"/>s contained in the <see cref="IBlockTableRecord"/>.
    /// </summary>
    public IObjectIdCollection ObjectIds { get; }
}