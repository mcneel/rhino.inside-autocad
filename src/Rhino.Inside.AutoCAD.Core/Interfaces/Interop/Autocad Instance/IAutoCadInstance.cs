namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Models the host Autocad Application.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public interface IAutoCadInstance
{
    /// <summary>
    /// Event raised when the Autocad document changes, e.g. a new document is opened.
    /// </summary>
    event EventHandler? DocumentCreated;

    /// <summary>
    /// Event raised when the units of the Autocad document change.
    /// </summary>
    event EventHandler? UnitsChanged;

    /// <summary>
    /// Event raised when the Autocad document is modified, eg. objects are added or removed.
    /// </summary>
    event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <summary>
    /// The <see cref="IValidationLogger"/> for this <see cref="IAutoCadInstance"/>.
    /// </summary>
    IValidationLogger ValidationLogger { get; }

    /// <summary>
    /// The list of open <see cref="IAutocadDocument"/>s in the AutoCAD application.
    /// </summary>
    List<IAutocadDocument> Documents { get; }

    /// <summary>
    /// The current active <see cref="IAutocadDocument"/> in the AutoCAD application.
    /// </summary>
    IAutocadDocument? ActiveDocument { get; }

    /// <summary>
    /// The version of the AutoCAD application.
    /// </summary>
    Version ApplicationVersion { get; }

    /// <summary>
    /// Ensures that the AutoCAD instance is properly shutdown and resources are released.
    /// </summary>
    void Shutdown();
}