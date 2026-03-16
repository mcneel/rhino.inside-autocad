using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A base register class for managing collections of AutoCAD database objects.
/// </summary>
/// <inheritdoc cref="IRegister{T}"/>
public abstract class RegisterBase<T> : IRegister<T> where T : INamedDbObject
{
    protected readonly Dictionary<IObjectId, T> _objects;
    protected readonly IAutocadDocument _document;
    protected bool _disposed;

    /// <summary>
    /// Constructs a new register for the specified AutoCAD document and populates it.
    /// </summary>
    protected RegisterBase(IAutocadDocument autocadDocument)
    {
        _objects = new Dictionary<IObjectId, T>(new ObjectIdEqualityComparer());
        _document = autocadDocument;

        this.Repopulate();
    }

    /// <summary>
    /// Clears this register of all values.
    /// </summary>
    protected void Clear()
    {
        _objects.Clear();
    }

    /// <inheritdoc />
    public abstract void Repopulate();

    /// <inheritdoc/>
    public bool TryGetByName(string name, out T? dbObject)
    {
        dbObject = _objects.FirstOrDefault(block => block.Value.Name.Equals(name)).Value;

        return dbObject != null;
    }

    /// <inheritdoc/>
    public bool TryGetById(IObjectId id, out T? blockTableRecord)
    {
        return _objects.TryGetValue(id, out blockTableRecord);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _objects.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Disposes of the <see cref="IRegister{T}"/> and all contained
    /// <see cref="INamedDbObject"/>s.
    /// </summary>
    protected void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            foreach (var blockTableRecord in _objects.Values)
                blockTableRecord.Dispose();

            _disposed = true;
        }
    }

    /// <summary>
    /// Public implementation of Dispose pattern callable by consumers.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }
}