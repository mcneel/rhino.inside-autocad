namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Base interface for all wrapped AutoCAD database objects (DBObject).
/// </summary>
/// <remarks>
/// Provides common properties shared by all AutoCAD database objects including entities,
/// symbol table records, and dictionaries. This is the root interface for the wrapper
/// hierarchy and is implemented by <see cref="IEntity"/>, <see cref="IAutocadLayerTableRecord"/>,
/// <see cref="IAutocadBlockTableRecord"/>, and other database object wrappers. Used by
/// Grasshopper components such as AutocadDbObjectComponent and AutocadObjectIdComponent.
/// </remarks>
/// <seealso cref="IObjectId"/>
/// <seealso cref="IEntity"/>
public interface IDbObject : IDisposable
{
    /// <summary>
    /// Gets the <see cref="IObjectId"/> that uniquely identifies this object in the database.
    /// </summary>
    /// <remarks>
    /// The ObjectId remains valid for the lifetime of the database session but may change
    /// after save/reload. Use the handle for persistent identification across sessions.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    IObjectId Id { get; }

    /// <summary>
    /// Gets the <see cref="System.Type"/> of the underlying AutoCAD DBObject.
    /// </summary>
    /// <remarks>
    /// Returns the runtime type of the wrapped AutoCAD object (e.g., Line, Circle,
    /// LayerTableRecord), useful for type checking and filtering operations.
    /// </remarks>
    Type Type { get; }

    /// <summary>
    /// Gets a value indicating whether this object reference is still valid.
    /// </summary>
    /// <remarks>
    /// Returns <c>false</c> if the underlying AutoCAD object has been erased or the
    /// database has been closed. Always check validity before accessing object properties
    /// to avoid exceptions.
    /// </remarks>
    bool IsValid { get; }

    /// <summary>
    /// Creates a new wrapper instance referencing the same underlying AutoCAD object.
    /// </summary>
    /// <returns>
    /// A new <see cref="IDbObject"/> wrapper sharing the same AutoCAD database object.
    /// </returns>
    /// <remarks>
    /// The cloned wrapper is independent but references the same AutoCAD object.
    /// Modifications through either wrapper affect the same underlying object.
    /// </remarks>
    IDbObject ShallowClone();
}
