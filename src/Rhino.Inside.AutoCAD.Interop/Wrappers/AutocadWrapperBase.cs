namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Abstract base class for wrapping AutoCAD value types and non-disposable objects.
/// </summary>
/// <typeparam name="T">
/// The type of the wrapped object.
/// </typeparam>
/// <remarks>
/// Use this base class for wrapping AutoCAD types that do not require disposal, such as
/// <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/> (a value type) or
/// <see cref="Autodesk.AutoCAD.EditorInput.SelectionFilter"/>. For wrapping database objects
/// that implement <see cref="IDisposable"/>, use <see cref="AutocadWrapperDisposableBase{T}"/> instead.
/// Derived classes include <see cref="AutocadObjectIdWrapper"/> and
/// <see cref="AutocadSelectionFilterWrapper"/>.
/// </remarks>
/// <seealso cref="AutocadWrapperDisposableBase{T}"/>
/// <seealso cref="AutocadObjectIdWrapper"/>
public abstract class AutocadWrapperBase<T>
{
    /// <summary>
    /// The wrapped AutoCAD object or value instance.
    /// </summary>
    protected T _wrappedAutocadObject;

    /// <summary>
    /// Gets the underlying AutoCAD object or value.
    /// </summary>
    /// <remarks>
    /// Provides direct access to the wrapped object for interop scenarios.
    /// Excluded from JSON serialization to prevent circular references or
    /// serialization of AutoCAD types. Use <see cref="InteropConverter"/>
    /// extension methods for type-safe unwrapping.
    /// </remarks>
    /// <seealso cref="InteropConverter"/>
    public T AutocadObject => _wrappedAutocadObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadWrapperBase{T}"/> class.
    /// </summary>
    /// <param name="autocadObject">
    /// The AutoCAD object or value to wrap.
    /// </param>
    protected AutocadWrapperBase(T autocadObject)
    {
        _wrappedAutocadObject = autocadObject;
    }
}
