using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IEntitySet"/>
public class EntitySet : IEntitySet
{
    private readonly List<IEntity> _entities = new();

    /// <summary>
    /// Constructs a new instance of <see cref="EntitySet"/>.
    /// </summary>
    public EntitySet() { }

    ///<inheritdoc/>
    public void Add(IEntity entity)
    {
        _entities.Add(entity);
    }

    ///<inheritdoc/>
    public IEnumerator<IEntity> GetEnumerator() => _entities.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}