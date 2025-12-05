using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The base super-interface for all entities.
/// </summary>
public interface IEntity : IDbObject
{
    /// <summary>
    /// The axis-aligned <see cref="BoundingBox"/> of the
    /// <see cref="IEntity"/>.
    /// </summary>
    BoundingBox BoundingBox { get; }

    /// <summary>
    /// Returns the name of this <see cref="IEntity"/>'s host <see cref="IAutocadLayer"/>.
    /// </summary>
    string LayerName { get; }

    /// <summary>
    /// The name of the underlying type of the <see cref="IEntity"/>.
    /// </summary>
    string TypeName { get; }
}