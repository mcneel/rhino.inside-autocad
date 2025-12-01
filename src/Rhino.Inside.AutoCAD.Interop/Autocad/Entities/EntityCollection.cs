using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IEntityCollection"/>
public class EntityCollection : IEntityCollection
{
    //private readonly InternalGeometryConverter _geometryConverter = InternalGeometryConverter.Instance!;
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

    /*    ///<inheritdoc/>
        public void TransformBy(ITransform transform)
        {
            var matrix3d = _geometryConverter.ToCadType(transform);

            foreach (var entity in _entities)
            {
                var cadEntity = entity.Unwrap();

                cadEntity.TransformBy(matrix3d);
            }
        }*/

    ///<inheritdoc/>
    public IEnumerator<IEntity> GetEnumerator() => _entities.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}