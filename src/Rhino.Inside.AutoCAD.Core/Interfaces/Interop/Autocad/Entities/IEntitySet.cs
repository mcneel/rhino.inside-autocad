namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a mutable collection of <see cref="IEntity"/> objects.
/// </summary>
/// <remarks>
/// Provides enumeration and accumulation of AutoCAD entities. Returned by methods like
/// <see cref="IAutocadBlockTableRecord.GetObjects"/> when extracting entities from a
/// block definition. Supports adding individual entities or merging collections.
/// </remarks>
/// <seealso cref="IEntity"/>
/// <seealso cref="IAutocadBlockTableRecord"/>
public interface IEntitySet : IEnumerable<IEntity>
{
    /// <summary>
    /// Adds an entity to this collection.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="IEntity"/> to add.
    /// </param>
    void Add(IEntity entity);
}