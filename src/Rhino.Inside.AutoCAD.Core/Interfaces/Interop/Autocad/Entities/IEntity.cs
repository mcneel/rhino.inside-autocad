namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The base super-interface for all entities.
/// </summary>
public interface IEntity : IDbObject
{
    /// <summary>
    /// The axis-aligned <see cref="IBoundingBox3d"/> of the
    /// <see cref="IEntity"/>.
    /// </summary>
   // IBoundingBox3d BoundingBox { get; }

    /// <summary>
    /// Returns the name of this <see cref="IEntity"/>'s host <see cref="ILayer"/>.
    /// </summary>
    string LayerName { get; }

    /// <summary>
    /// The name of the underlying type of the <see cref="IEntity"/>.
    /// </summary>
    string TypeName { get; }
}