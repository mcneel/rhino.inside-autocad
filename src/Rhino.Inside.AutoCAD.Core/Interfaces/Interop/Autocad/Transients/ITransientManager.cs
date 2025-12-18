namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which wraps an AutoCAD TransientManager providing
/// functionality for managing transient entities.
/// </summary>
public interface ITransientManager : IDisposable
{
    /// <summary>
    /// Adds the provided <paramref name="entity"/> into this <see cref="ITransientManager"/>.
    /// </summary>
    void AddEntity(IEntity entity);

    /// <summary>
    /// Adds the provided <paramref name="entities"/> into this <see cref="ITransientManager"/>.
    /// </summary>
    void AddEntities(IEnumerable<IEntity> entities);

    /// <summary>
    /// Removes the provided <paramref name="entity"/> from this <see cref="ITransientManager"/>.
    /// </summary>
    void RemoveEntity(IEntity entity);

    /// <summary>
    /// Removes the provided <paramref name="entities"/> from this <see cref="ITransientManager"/>.
    /// </summary>
    void RemoveEntities(IEnumerable<IEntity> entities);
}