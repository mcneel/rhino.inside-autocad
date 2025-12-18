namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of <see cref="IEntity"/>s.
/// </summary>
public interface IEntityCollection : IEnumerable<IEntity>
{
    /// <summary>
    /// Adds an <see cref="IEntity"/> to the <see cref="IEntityCollection"/>.
    /// </summary>
    void Add(IEntity entity);

    /// <summary>
    /// Adds a collection of <see cref="IEntity"/>s to the <see cref="IEntityCollection"/>.
    /// </summary>
    void Add(IEntityCollection entityCollection);
}