namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD document (DWG file) and provides access to its database,
/// repositories, and transactional operations.
/// </summary>
/// <remarks>
/// This interface wraps an AutoCAD Document object and exposes functionality needed
/// by Rhino.Inside.AutoCAD components. It manages event subscriptions for document
/// changes and provides register access for layers, layouts, line types, and blocks.
/// Used extensively by Grasshopper components such as AutocadDocumentComponent and
/// GetAutocadDocumentsComponent for document-level operations.
/// </remarks>
/// <seealso cref="IAutocadDatabase"/>
/// <seealso cref="IAutocadDocumentChangeEventArgs"/>
public interface IAutocadDocument
{
    /// <summary>
    /// Occurs when a command within this document completes successfully.
    /// </summary>
    /// <remarks>
    /// This event only fires when a command concludes successfully, not when cancelled.
    /// For example, if a user creates a polyline and presses "Esc" after the final vertex,
    /// the command is cancelled and this event will not fire. Pressing "Enter" completes
    /// the command and triggers this event.
    /// </remarks>
    event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <summary>
    /// Gets the unique identifier for this document instance.
    /// </summary>
    /// <remarks>
    /// This ID is generated when the wrapper is created and remains constant for the
    /// lifetime of this instance. It is distinct from the AutoCAD document handle.
    /// </remarks>
    IAutocadDocumentId DocumentId { get; }

    /// <summary>
    /// Gets the <see cref="IAutocadDatabase"/> containing all objects in this document.
    /// </summary>
    /// <remarks>
    /// The database provides low-level access to AutoCAD's object storage and is used
    /// for operations that require direct database manipulation.
    /// </remarks>
    IAutocadDatabase AutocadDatabase { get; }

    /// <summary>
    /// Gets file information for this document, including path and name.
    /// </summary>
    IAutocadDocumentFileMetadata FileMetadata { get; }

    /// <summary>
    /// Gets the <see cref="UnitSystem"/> representing the drawing units of this document.
    /// </summary>
    /// <remarks>
    /// Returns the Rhino UnitSystem equivalent of the AutoCAD INSUNITS setting.
    /// Important for geometry conversion between Rhino and AutoCAD coordinate systems.
    /// </remarks>
    UnitSystem UnitSystem { get; }

    /// <summary>
    /// Re-reads the drawing units from the document and returns them.
    /// </summary>
    /// <remarks>
    /// <see cref="UnitSystem"/> is cached and only refreshed when a command ends having made
    /// other changes, so a drawing whose INSUNITS was changed on its own can still report the
    /// units it was opened with. Reading the header costs nothing and needs no transaction, so
    /// callers that must not be wrong - rather than merely up to date - ask through here. The
    /// cached value is updated too, so every other reader agrees with the returned one.
    /// </remarks>
    UnitSystem RefreshUnitSystem();

    /// <summary>
    /// A boolean flag indicating whether this document is read-only.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Creates a new transaction manager for performing database operations within this document.
    /// </summary>
    public IAutocadTransactionManager CreateTransactionManager();

    /// <summary>
    /// Forces a screen refresh to display pending visual changes.
    /// </summary>
    /// <remarks>
    /// Call this after modifying geometry or visual properties to ensure the
    /// viewport reflects the current state of the document.
    /// </remarks>
    void UpdateEditorScreen();

    /// <summary>
    /// Regenerates the document to update all computed geometry and display.
    /// </summary>
    /// <remarks>
    /// Performs a full regeneration (REGEN command equivalent) to recalculate
    /// geometry, update block references, and refresh associative networks.
    /// More thorough than <see cref="UpdateEditorScreen"/> but also more expensive.
    /// </remarks>
    /// <seealso cref="UpdateEditorScreen"/>
    void Regenerate();

    /// <summary>
    /// Retrieves a database object by its <see cref="IObjectId"/>.
    /// </summary>
    /// <param name="objectId">
    /// The object identifier to look up.
    /// </param>
    /// <returns>
    /// The <see cref="IDbObject"/> corresponding to the specified ID.
    /// </returns>
    /// <remarks>
    /// Used by Grasshopper components such as GetByAutocadIdComponent to retrieve
    /// objects from the database. The returned object provides access to common
    /// database object properties and methods.
    /// </remarks>
    /// <seealso cref="IObjectId"/>
    /// <seealso cref="IDbObject"/>
    IDbObject? GetObjectById(IObjectId objectId);

    /// <summary>
    /// Retrieves a database object by its handle value.
    /// </summary>
    /// <param name="handle">
    /// The numeric handle value of the object.
    /// </param>
    /// <returns>
    /// The <see cref="IDbObject"/> if found; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Handle values are persistent across save/load cycles, unlike ObjectIds.
    /// Used by GetAutocadByHandleComponent to locate objects by their handle string.
    /// </remarks>
    /// <seealso cref="IDbObject"/>
    IDbObject? GetObjectByHandle(long handle);

    /// <summary>
    /// Closes this document wrapper and unsubscribes from all events.
    /// </summary>
    /// <remarks>
    /// This method unhooks from database and document events to prevent memory leaks
    /// and invalid callbacks. Call this when the document wrapper is no longer needed.
    /// This does not close the underlying AutoCAD document.
    /// </remarks>
    void CloseDocument();
}
