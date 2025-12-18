using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IEntityCollection"/>
public class EntityCollection : IEntityCollection
{
    private readonly List<IEntity> _entities = new();

    /// <summary>
    /// Constructs a new instance of <see cref="EntityCollection"/>.
    /// </summary>
    public EntityCollection() { }

    ///<inheritdoc/>
    public void Add(IEntity entity)
    {
        _entities.Add(entity);
    }

    ///<inheritdoc/>
    public void Add(IEntityCollection entityCollection)
    {
        foreach (var entity in entityCollection) this.Add(entity);
    }

    ///<inheritdoc/>
    public IEnumerator<IEntity> GetEnumerator() => _entities.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}