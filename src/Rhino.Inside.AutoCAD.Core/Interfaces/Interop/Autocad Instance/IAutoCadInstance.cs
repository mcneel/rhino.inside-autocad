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
    /// Event raised when the <see cref="IAutocadDocument"/> begins closing , or when the
    /// user changes the active document. If either event occurs, the application will
    /// close.
    /// </summary>
    /// <remarks>
    /// The event triggers the AWI application to shutdown. This ensures that only one
    /// instance of the AWI application is running per AutoCAD session, thereby resolving
    /// any complications that can arise if multiple instances were allowed. For example,
    /// singleton access to the Geometry Converter is easier to manage
    /// and reason about when this policy is enforced, plus it prevents cross-cutting
    /// concerns from impacting negatively on the quality of the codebase. Additionally,
    /// automated actions such as disabling the AutoCAD ribbon menu while the AWI
    /// application is running can be bypassed (reactivated) by launching another
    /// instance of the AWI application via a different document. The single-instance
    /// approach therefore simplifies interaction between AutoCAD and the AWI application.
    /// </remarks>
    event EventHandler? DocumentClosingOrActivated;

    /// <summary>
    /// The <see cref="IValidationLogger"/> for this <see cref="IAutoCadInstance"/>.
    /// </summary>
    IValidationLogger ValidationLogger { get; }

    List<IAutocadDocument> Documents { get; }

    /// <summary>
    /// The current active <see cref="IAutocadDocument"/> in the AutoCAD application.
    /// </summary>
    IAutocadDocument? ActiveDocument { get; }
}