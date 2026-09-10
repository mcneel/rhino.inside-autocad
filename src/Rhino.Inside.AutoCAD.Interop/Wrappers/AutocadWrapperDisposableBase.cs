namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Abstract base class for wrapping AutoCAD objects that implement <see cref="IDisposable"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the wrapped object, which must implement <see cref="IDisposable"/>.
/// </typeparam>
/// <remarks>
/// Provides the standard dispose pattern for wrapper classes that manage AutoCAD database objects
/// such as <see cref="Autodesk.AutoCAD.DatabaseServices.DBObject"/> and its derivatives.
/// When disposed, the underlying AutoCAD object is also disposed.
/// </remarks>
/// <remarks>
/// Only derive from this when the wrapper genuinely <em>owns</em> the object it wraps - one this
/// code created and nothing else will close. AutoCAD owns everything else: a document's
/// <see cref="Autodesk.AutoCAD.DatabaseServices.Database"/>, an object opened by a transaction,
/// and the objects handed out by database reactors are all closed by AutoCAD itself and must not
/// be closed again. Wrappers for those derive from
/// <see cref="AutocadWrapperBase{T}"/> instead. The non-database-resident entities built for
/// transient previews are the main thing this base is right for.
/// </remarks>
/// <seealso cref="AutocadWrapperBase{T}"/>
/// <seealso cref="AutocadDbObjectWrapper"/>
public abstract class AutocadWrapperDisposableBase<T> : IDisposable where T : IDisposable
{
    /// <summary>
    /// Indicates whether this wrapper has been disposed.
    /// </summary>
    private protected bool _disposed;

    /// <summary>
    /// The wrapped AutoCAD object instance.
    /// </summary>
    protected T _wrappedAutocadObject;

    /// <summary>
    /// Gets the underlying AutoCAD object.
    /// </summary>
    /// <remarks>
    /// Provides direct access to the wrapped object for interop scenarios.
    /// Excluded from JSON serialization to prevent circular references.
    /// Use <see cref="InteropConverter"/> extension methods for type-safe unwrapping.
    /// </remarks>
    /// <seealso cref="InteropConverter"/>
    public T AutocadObject => _wrappedAutocadObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadWrapperDisposableBase{T}"/> class.
    /// </summary>
    /// <param name="autocadObject">
    /// The AutoCAD object to wrap.
    /// </param>
    protected AutocadWrapperDisposableBase(T autocadObject)
    {
        _wrappedAutocadObject = autocadObject;
    }

    /// <summary>
    /// Releases managed resources when disposing.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> if called from <see cref="Dispose()"/>; <c>false</c> if called from a finalizer.
    /// </param>
    /// <remarks>
    /// Override this method in derived classes to release additional managed resources.
    /// Always call <c>base.Dispose(disposing)</c> after releasing derived class resources.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _wrappedAutocadObject.Dispose();

            _disposed = true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Disposes the wrapped AutoCAD object and suppresses finalization.
    /// After calling this method, the wrapper should not be used.
    /// </remarks>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }
}
