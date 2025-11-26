using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// The base wrapper class which implements <see cref="IDisposable"/>.
/// </summary>
public abstract class WrapperDisposableBase<T> : IDisposable where T : IDisposable
{
    private protected bool _disposed;

    protected T _wrappedValue;

    /// <summary>
    /// The underlying entity.
    /// </summary>
    [JsonIgnore]
    public T Internal => _wrappedValue;

    /// <summary>
    /// Constructs a new <see cref="WrapperDisposableBase{T}"/>.
    /// </summary>
    protected WrapperDisposableBase(T value)
    {
        _wrappedValue = value;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _wrappedValue.Dispose();

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