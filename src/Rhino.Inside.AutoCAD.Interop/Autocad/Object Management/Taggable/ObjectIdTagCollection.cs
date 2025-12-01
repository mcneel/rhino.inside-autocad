using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IObjectIdTagCollection"/>
public class ObjectIdTagCollection : IObjectIdTagCollection
{
    private readonly Dictionary<IEntity, IObjectIdTag> _tags = new();

    // To detect redundant calls
    private bool _disposedValue;

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTagCollection"/>.
    /// </summary>
    public ObjectIdTagCollection(IEntityCollection entities)
    {
        foreach (var entity in entities)
        {
            this.Add(entity);
        }
    }

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTagCollection"/>.
    /// </summary>
    public ObjectIdTagCollection(IList<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.Add(entity);
        }
    }

    /// <summary>
    /// Adds an <see cref="IObjectIdTag"/> of the requested <see cref="IEntity"/> to the
    /// <see cref="IObjectIdTagCollection"/>.
    /// </summary>
    private void Add(IEntity entity)
    {
        if (_tags.ContainsKey(entity)) return;

        var objectId = entity.Id;

        var objectIdTag = new ObjectIdTag(objectId);

        _tags.Add(entity, objectIdTag);
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    protected void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing)
        {
            foreach (var entity in _tags.Keys)
            {
                entity.Dispose();
            }
        }

        _disposedValue = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerator<IObjectIdTag> GetEnumerator() => _tags.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}